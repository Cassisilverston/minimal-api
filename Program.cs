using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Db;
using Scalar.AspNetCore;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authorization;




#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration["Jwt"] ?? "123456";
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
   option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
   option.TokenValidationParameters = new TokenValidationParameters
   {
      ValidateLifetime = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
      ValidateIssuer = false,
      ValidateAudience = false,
   };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministratorServices, AdministratorService>();

builder.Services.AddScoped<IVehicleServices, VehicleService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter only the JWT token."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        if (!document.Components.SecuritySchemes.ContainsKey("Bearer"))
        {
            document.Components.SecuritySchemes.Add("Bearer", securityScheme);
        }
        document.Security ??= new List<OpenApiSecurityRequirement>();
        
        var requirement = new OpenApiSecurityRequirement();

        var schemeReference = new OpenApiSecuritySchemeReference("Bearer", document);
        
        requirement.Add(schemeReference, new List<string>());
        
        document.Security.Add(requirement);

        return Task.CompletedTask;
    });
});



builder.Services.AddDbContext<AppDbContext>(options =>
{
   options.UseMySql(builder.Configuration.GetConnectionString("MySql"),
   ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))
   );
});

var app = builder.Build();
#endregion


#region Home
app.MapGet("/", () => Results.Ok(new Home())).AllowAnonymous().WithTags("Home");
#endregion


#region Administrator
string GenerateTokenJwt(Administrator administrator)
{
   if (string.IsNullOrEmpty(key)) return string.Empty;

   var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
   var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

   var claims = new List<Claim>()
   {
      new Claim("Email", administrator.Email),
      new Claim("Profile", administrator.Profile),
      new Claim(ClaimTypes.Role, administrator.Profile)
   };

   var token = new JwtSecurityToken(
      claims: claims,
      expires: DateTime.Now.AddDays(1),
      signingCredentials: credentials
   );

   return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorServices administratorServices) =>
{
   var adm = administratorServices.Login(loginDTO);
   if (adm != null)
   {
      string token = GenerateTokenJwt(adm);

      return Results.Ok(new AdministratorLogged
      {
         Email = adm.Email,
         Profile = adm.Profile,
         Token = token
      });
   }
   else
      return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administrators");

app.MapGet("/administrators", ([FromQuery] int? page, IAdministratorServices administratorServices) =>
{
   var adms = new List<AdministratorModelView>();

   var administrators = administratorServices.All(page);

   foreach (var adm in administrators)
   {
      adms.Add(new AdministratorModelView
      {
         Id = adm.Id,
         Email = adm.Email,
         Profile = adm.Profile
      });
   }

   return Results.Ok(adms);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
.WithTags("Administrators");

app.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorServices administratorServices) =>
{
   var administrator = administratorServices.SearchById(id);

   if (administrator == null) return Results.NotFound("This vehicle doesn't exist!");

   return Results.Ok(new AdministratorModelView
   {
      Id = administrator.Id,
      Email = administrator.Email,
      Profile = administrator.Profile
   });
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
.WithTags("Administrators");

app.MapPost("administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorServices administratorServices) =>
{
   var validation = new ValidationErrors
   {
      Messages = new List<string>()
   };

   if (string.IsNullOrEmpty(administratorDTO.Email))
   {
      validation.Messages.Add("The email cannot be empty");
   }

   if (string.IsNullOrEmpty(administratorDTO.Password))
   {
      validation.Messages.Add("The password cannot be empty");
   }

   if (administratorDTO.Profile == null)
   {
      validation.Messages.Add("The profile cannot be empty");
   }

   if (validation.Messages.Count > 0)
   {
      return Results.BadRequest(validation);
   }

   var administrator = new Administrator
   {
      Email = administratorDTO.Email,
      Password = administratorDTO.Password,
      Profile = administratorDTO.Profile.ToString() ?? Profile.Editor.ToString()
   };

   administratorServices.Add(administrator);

   return Results.Created($"/administrator/{administrator.Id}", new AdministratorModelView
   {
      Id = administrator.Id,
      Email = administrator.Email,
      Profile = administrator.Profile
   });

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
.WithTags("Administrators");
#endregion


#region Vehicles
ValidationErrors ValidDTO(VehicleDTO vehicleDTO)
{
   var validation = new ValidationErrors
   {
      Messages = new List<string>()
   };

   if (string.IsNullOrEmpty(vehicleDTO.Name))
   {
      validation.Messages.Add("The name cannot be empty");
   }

   if (string.IsNullOrEmpty(vehicleDTO.Mark))
   {
      validation.Messages.Add("The mark cannot be empty");
   }

   if (vehicleDTO.Year < 1886)
   {
      validation.Messages.Add($"There is no car inferior to this one.The world's first car with an internal combustion engine, the Benz Patent-Motorwagen, was created by the German inventor Karl Benz in 1885 and patented on January 29, 1886.");
   }
   return validation;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
{
   var validation = ValidDTO(vehicleDTO);
   if (validation.Messages.Count > 0)
   {
      return Results.BadRequest(validation);
   }


   var vehicle = new Vehicle
   {
      Name = vehicleDTO.Name,
      Mark = vehicleDTO.Mark,
      Year = vehicleDTO.Year
   };

   vehicleServices.Create(vehicle);

   return Results.Created($"/vehicle/{vehicle.Id}", vehicle);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm, Editor"})
.WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleServices vehicleServices) =>
{
   var vehicle = vehicleServices.All(page);

   return Results.Ok(vehicle);

}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) =>
{
   var vehicle = vehicleServices.SearchById(id);

   if (vehicle == null) return Results.NotFound("This vehicle doesn't exist!");

   return Results.Ok(vehicle);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm, Editor"})
.WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
{
   var vehicle = vehicleServices.SearchById(id);
   if (vehicle == null) return Results.NotFound("This vehicle doesn't exist!");

   var validation = ValidDTO(vehicleDTO);
   if (validation.Messages.Count > 0)
   {
      return Results.BadRequest(validation);
   }

   vehicle.Name = vehicleDTO.Name;
   vehicle.Mark = vehicleDTO.Mark;
   vehicle.Year = vehicleDTO.Year;

   vehicleServices.Update(vehicle);

   return Results.Ok(vehicle);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
.WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) =>
{
   var vehicle = vehicleServices.SearchById(id);

   if (vehicle == null) return Results.NotFound("This vehicle doesn't exist!");

   vehicleServices.DeleteByVehicle(vehicle);

   return Results.NoContent();

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
.WithTags("Vehicles");
#endregion


#region App
if (app.Environment.IsDevelopment())
{
   app.MapOpenApi();
   app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion

