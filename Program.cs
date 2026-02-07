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


#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorServices, AdministratorService>();

builder.Services.AddScoped<IVehicleServices, VehicleService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();


builder.Services.AddDbContext<AppDbContext>(options =>
{
   options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
   ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
   );
});

var app = builder.Build();
#endregion


#region Home
app.MapGet("/", () => Results.Ok(new Home())).WithTags("Home");
#endregion


#region Administrator
app.MapPost("/administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorServices administratorServices) =>
{
    if(administratorServices.Login(loginDTO) != null)
       return Results.Ok("Login com sucesso!");
    else
       return Results.Unauthorized();
}).WithTags("Administrators");

app.MapGet("/administrators", ([FromQuery] int? page, IAdministratorServices administratorServices) =>
{
   var adms = new List<AdministratorModelView>();

   var administrators = administratorServices.All(page);

   foreach(var adm in administrators)
   {
      adms.Add(new AdministratorModelView
      {
         Id = adm.Id,
         Email = adm.Email,
         Profile = adm.Profile
      });
   }

   return Results.Ok(adms);
}).WithTags("Administrators");

app.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorServices administratorServices) => {
   var administrator = administratorServices.SearchById(id);

   if(administrator == null) return Results.NotFound("This vehicle doesn't exist!");

   return Results.Ok(new AdministratorModelView
      {
         Id = administrator.Id,
         Email = administrator.Email,
         Profile = administrator.Profile
      });
}).WithTags("Administrators");

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

}).WithTags("Administrators");
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

app.MapPost("/vehicles", ([FromBody]VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
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

}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleServices vehicleServices) => {
   var vehicle = vehicleServices.All(page);

   return Results.Ok(vehicle);

}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) => {
   var vehicle = vehicleServices.SearchById(id);

   if(vehicle == null) return Results.NotFound("This vehicle doesn't exist!");

   return Results.Ok(vehicle);

}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleServices vehicleServices) => {
   var vehicle = vehicleServices.SearchById(id);
   if(vehicle == null) return Results.NotFound("This vehicle doesn't exist!");
   
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

}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) => {
   var vehicle = vehicleServices.SearchById(id);

   if(vehicle == null) return Results.NotFound("This vehicle doesn't exist!");

   vehicleServices.DeleteByVehicle(vehicle);

   return Results.NoContent();

}).WithTags("Vehicles");
#endregion


#region App
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();
#endregion
