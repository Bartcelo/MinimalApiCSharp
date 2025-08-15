using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Api.Dominio.DTOs;
using Minimal_Api.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>(options =>{
options.UseMySql(
    builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/login", (LoginDto loginDto) =>
loginDto.Email == "bart.celo@hotmail.com" && loginDto.Password == "123456"
    ? Results.Ok("Login realizado com sucesso")
    : Results.BadRequest("Acesso negado")
);

app.Run();


