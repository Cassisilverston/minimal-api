using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.Services;
using MinimalApi.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorServices, AdministratorService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
   options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
   ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
   );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody]LoginDTO loginDTO, IAdministratorServices administratorServices) =>
{
    if(administratorServices.Login(loginDTO) != null)
       return Results.Ok("Login com sucesso!");
    else
       return Results.Unauthorized();
});

app.Run();
