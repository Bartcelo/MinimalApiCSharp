using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Api.Dominio.DTOs;
using Minimal_Api.Dominio.Interfaces;
using Minimal_Api.Dominio.Services;
using Minimal_Api.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddDbContext<DbContexto>(options =>{
options.UseMySql(
    builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody]LoginDto loginDto , IAdministradorServico administradorServico) =>
administradorServico.Login(loginDto) != null
    ? Results.Ok("Login realizado com sucesso")
    : Results.BadRequest("Acesso negado")
);

app.Run();


