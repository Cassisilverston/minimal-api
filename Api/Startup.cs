using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MinimalApi;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Db;
using Scalar.AspNetCore;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        var key = Configuration["Jwt"] ?? "123456";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministratorServices, AdministratorService>();
        services.AddScoped<IVehicleServices, VehicleService>();

        services.AddEndpointsApiExplorer();

        services.AddOpenApi(options =>
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

        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            options.UseNpgsql(connectionString);
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        

        app.UseEndpoints(endpoints =>
        {
            if (env.IsDevelopment())
            {
                endpoints.MapOpenApi();
                endpoints.MapScalarApiReference();
            }

            #region Home
            endpoints.MapGet("/", () => Results.Ok(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Administrator
            endpoints.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorServices administratorServices) =>
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

            // Get All
            endpoints.MapGet("/administrators", ([FromQuery] int? page, IAdministratorServices administratorServices) =>
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
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administrators");

            // Get By Id
            endpoints.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorServices administratorServices) =>
            {
                var administrator = administratorServices.SearchById(id);
                if (administrator == null) return Results.NotFound("This administrator doesn't exist!");

                return Results.Ok(new AdministratorModelView
                {
                    Id = administrator.Id,
                    Email = administrator.Email,
                    Profile = administrator.Profile
                });
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administrators");

            // Create Administrator
            endpoints.MapPost("administrators", ([FromBody] AdministratorDTO administratorDTO, IAdministratorServices administratorServices) =>
            {
                var validation = new ValidationErrors { Messages = new List<string>() };

                if (string.IsNullOrEmpty(administratorDTO.Email)) validation.Messages.Add("The email cannot be empty");
                if (string.IsNullOrEmpty(administratorDTO.Password)) validation.Messages.Add("The password cannot be empty");
                if (administratorDTO.Profile == null) validation.Messages.Add("The profile cannot be empty");

                if (validation.Messages.Count > 0) return Results.BadRequest(validation);

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
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administrators");
            #endregion

            #region Vehicles
            // Create Vehicle
            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
            {
                var validation = ValidDTO(vehicleDTO);
                if (validation.Messages.Count > 0) return Results.BadRequest(validation);

                var vehicle = new Vehicle
                {
                    Name = vehicleDTO.Name,
                    Mark = vehicleDTO.Mark,
                    Year = vehicleDTO.Year
                };

                vehicleServices.Create(vehicle);
                return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
            .WithTags("Vehicles");

            // Get All Vehicles
            endpoints.MapGet("/vehicles", ([FromQuery] int? page, IVehicleServices vehicleServices) =>
            {
                var vehicle = vehicleServices.All(page);
                return Results.Ok(vehicle);
            })
            .RequireAuthorization()
            .WithTags("Vehicles");

            // Get Vehicle By Id
            endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) =>
            {
                var vehicle = vehicleServices.SearchById(id);
                if (vehicle == null) return Results.NotFound("This vehicle doesn't exist!");
                return Results.Ok(vehicle);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
            .WithTags("Vehicles");

            // Update Vehicle
            endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
            {
                var vehicle = vehicleServices.SearchById(id);
                if (vehicle == null) return Results.NotFound("This vehicle doesn't exist!");

                var validation = ValidDTO(vehicleDTO);
                if (validation.Messages.Count > 0) return Results.BadRequest(validation);

                vehicle.Name = vehicleDTO.Name;
                vehicle.Mark = vehicleDTO.Mark;
                vehicle.Year = vehicleDTO.Year;

                vehicleServices.Update(vehicle);
                return Results.Ok(vehicle);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Vehicles");

            // Delete Vehicle
            endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) =>
            {
                var vehicle = vehicleServices.SearchById(id);
                if (vehicle == null) return Results.NotFound("This vehicle doesn't exist!");

                vehicleServices.DeleteByVehicle(vehicle);
                return Results.NoContent();
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Vehicles");
            #endregion
        });
    }

    private string GenerateTokenJwt(Administrator administrator)
    {
        var key = Configuration["Jwt"] ?? "123456";

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

    private ValidationErrors ValidDTO(VehicleDTO vehicleDTO)
    {
        var validation = new ValidationErrors
        {
            Messages = new List<string>()
        };

        if (string.IsNullOrEmpty(vehicleDTO.Name))
            validation.Messages.Add("The name cannot be empty");

        if (string.IsNullOrEmpty(vehicleDTO.Mark))
            validation.Messages.Add("The mark cannot be empty");

        if (vehicleDTO.Year < 1886)
            validation.Messages.Add($"There is no car inferior to this one. The world's first car... 1886.");

        return validation;
    }
}
