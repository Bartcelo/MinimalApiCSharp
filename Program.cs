
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Minimal_Api.Dominio.DTOs;
using Minimal_Api.Dominio.Entidades;
using Minimal_Api.Dominio.Enuns;
using Minimal_Api.Dominio.Interfaces;
using Minimal_Api.Dominio.ModelViews;
using Minimal_Api.Dominio.Services;
using Minimal_Api.Infraestrutura.Db;


#region Buider
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var key = builder.Configuration.GetSection("Jwt").ToString();
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

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        });

        builder.Services.AddAuthorization();

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

        #region  Administradores

        string GerarTokenJwt(Administrador administrador)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new("Email", administrador.Email),
                new("Perfil", administrador.Perfil ),

            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }





        app.MapPost("/login", ([FromBody] LoginDto loginDto, IAdministradorServico administradorServico) =>
        {
            var adm = administradorServico.Login(loginDto);
            if (adm != null)
            {
                string token = GerarTokenJwt(adm);

                return Results.Ok(new AdiministradorLogado
                {
                    Email = adm.Email,
                    Perfil = adm.Perfil,
                    Token = token
                });

            }
            
                return Results.Unauthorized();
            

        }).WithTags("Login");

        app.MapPost("/administrador", ([FromBody] AdministradorDto administradorDto, IAdministradorServico administradorServico) =>
        {

            var validacao = new ErrosDeValidacao
            {
                Mensagens = new List<string>()
            };

            if (string.IsNullOrEmpty(administradorDto.Email))
                validacao.Mensagens.Add("O email não pode ser vazio ou nulo");
            if (string.IsNullOrEmpty(administradorDto.Senha))
                validacao.Mensagens.Add("A Senha não pode ser vazia ou nula");
            if (string.IsNullOrEmpty(administradorDto.Perfil.ToString()))
                validacao.Mensagens.Add("O Perfiel não pode ser vazio ou nulo");

            if (validacao.Mensagens.Count > 0)
                return Results.BadRequest(validacao);

            var administrador = new Administrador
            {
                Email = administradorDto.Email,
                Senha = administradorDto.Senha,
                Perfil = administradorDto.Perfil.ToString()

            };
            administradorServico.Incluir(administrador);


            var administradorId = new AdministradorModelView
            {
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil

            };

            return Results.Created($"/administrador/{administrador.Email}", administradorId);

        }

        ).RequireAuthorization().WithTags("Login");

        app.MapGet("/administradores", ([FromQuery] int? Pagina, IAdministradorServico administradorServico) =>
        {
            var administrador = new List<AdministradorModelView>();
            // if (Pagina == null) return Pagina = 1;
            int paginaAtual = Pagina ?? 1;
            var administradores = administradorServico.Todos(paginaAtual);

            foreach (var item in administradores)
            {
                administrador.Add(new AdministradorModelView
                {
                    Id = item.Id,
                    Email = item.Email,
                    Perfil = item.Perfil

                });

            }


            return Results.Ok(administrador);


        }).RequireAuthorization().WithTags("Login");


        app.MapGet("/administrador/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
        {
            var administrador = administradorServico.BuscarPorId(id);

            if (administrador == null) return Results.NotFound();

            var administradorId = new AdministradorModelView
            {
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil

            };

            return Results.Ok(administradorId);

        }).RequireAuthorization().WithTags("Login");

        #endregion

        #region Veiculos
        ErrosDeValidacao validaDTO(VeiculoDto veiculoDto)
        {

            var validacao = new ErrosDeValidacao
            {
                Mensagens = []
            };

            if (string.IsNullOrEmpty(veiculoDto.Nome))
                validacao.Mensagens.Add("O nome não é valido");
            if (string.IsNullOrEmpty(veiculoDto.Marca))
                validacao.Mensagens.Add("O Marca não é valida");
            if (veiculoDto.Ano < 1950)
                validacao.Mensagens.Add("O ano do veiculo deve ser  superior a 1950");


            return validacao;

        }

        app.MapPost("/veiculo", ([FromBody] VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
        {

            var validacao = validaDTO(veiculoDto);
            if (validacao.Mensagens.Count > 0)
                return Results.BadRequest(validacao);

            var veiculo = new Veiculo
            {
                Nome = veiculoDto.Nome,
                Marca = veiculoDto.Marca,
                Ano = veiculoDto.Ano

            };
            veiculoServico.Incluir(veiculo);

            return Results.Created($"/veiculo/{veiculo.Id}", veiculo);



        }).RequireAuthorization().WithTags("Veiculos");

        app.MapGet("/veiculos", (int? Pagina, IVeiculoServico veiculoServico) =>
        {

            if (Pagina == null)
            {
                Pagina = 1;
            }

            var veiculos = veiculoServico.Todos((int)Pagina);
            return Results.Ok(veiculos);

        }).RequireAuthorization().WithTags("Veiculos");

        app.MapGet("/veiculo/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
        {
            var veiculo = veiculoServico.BuscarPorId(id);

            if (veiculo == null) return Results.NotFound();

            return Results.Ok(veiculo);

        }).RequireAuthorization().WithTags("Veiculos");


        app.MapPut("/veiculo/{id}", ([FromRoute] int id, VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
        {
            var veiculo = veiculoServico.BuscarPorId(id);
            if (veiculo == null) return Results.NotFound();

            var validacao = validaDTO(veiculoDto);
            if (validacao.Mensagens.Count > 0)
                return Results.BadRequest(validacao);

            veiculo.Nome = veiculoDto.Nome;
            veiculo.Marca = veiculoDto.Marca;
            veiculo.Ano = veiculoDto.Ano;

            veiculoServico.Atualizar(veiculo);

            return Results.Ok(veiculo);

        }).RequireAuthorization().WithTags("Veiculos");




        app.MapDelete("/veiculo/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
        {
            var veiculo = veiculoServico.BuscarPorId(id);

            if (veiculo == null) return Results.NotFound();

            veiculoServico.Apagar(veiculo);

            return Results.NoContent();

        }).RequireAuthorization().WithTags("Veiculos");

        #endregion

        #region  App
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.Run();
    }
}

#endregion


