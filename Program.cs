using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Minimal_Api.Dominio.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDto loginDto) =>
loginDto.Email == "bart.celo@hotmail.com" && loginDto.Password == "123456"  
    ? Results.Ok("Login realizado com sucesso") 
    : Results.BadRequest("Acesso negado")
);

app.Run();


