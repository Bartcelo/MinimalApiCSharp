
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Api.Dominio.DTOs;
using Minimal_Api.Dominio.Entidades;
using Minimal_Api.Dominio.Interfaces;
using Minimal_Api.Dominio.ModelViews;
using Minimal_Api.Dominio.Services;
using Minimal_Api.Infraestrutura.Db;


#region Buider
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
        );
});


var app = builder.Build();
#endregion

#region  Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region  Login
app.MapPost("/login", ([FromBody] LoginDto loginDto, IAdministradorServico administradorServico) =>
administradorServico.Login(loginDto) != null
    ? Results.Ok("Login realizado com sucesso")
    : Results.BadRequest("Acesso negado")
).WithTags("Login");
#endregion


#region Veiculos
app.MapPost("/veiculo", ([FromBody] VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
{

    var veiculo = new Veiculo
    {
        Nome = veiculoDto.Nome,
        Marca = veiculoDto.Marca,
        Ano = veiculoDto.Ano

    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);



}).WithTags("Veiculos");

app.MapGet("/veiculos", (int? Pagina, IVeiculoServico veiculoServico) =>
{

    if (Pagina == null)
    {
        Pagina = 1;
    }

    var veiculos = veiculoServico.Todos((int)Pagina);
    return Results.Ok(veiculos);

}).WithTags("Veiculos");

app.MapGet("/veiculo/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);

}).WithTags("Veiculos");


app.MapPut("/veiculo/{id}", ([FromRoute]int id, VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null) return Results.NotFound();

    veiculo.Nome = veiculoDto.Nome;
    veiculo.Marca = veiculoDto.Marca;
    veiculo.Ano = veiculoDto.Ano;

    veiculoServico.Atualizar(veiculo);
   
    return Results.Ok(veiculo);

}).WithTags("Veiculos");

#endregion



#region  App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion

