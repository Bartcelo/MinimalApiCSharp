using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal_Api.Dominio.DTOs;
using Minimal_Api.Dominio.Entidades;
using Minimal_Api.Dominio.Interfaces;
using Minimal_Api.Dominio.ModelViews;
using Minimal_Api.Dominio.Services;
using Minimal_Api.Infraestrutura.Db;

namespace Minimal_Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration.GetSection("Jwt").ToString();
        }

        private string key;

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(option =>
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
                         ValidateAudience = false

                     };
                 });

            services.AddAuthorization();

            services.AddScoped<IAdministradorServico, AdministradorServico>();
            services.AddScoped<IVeiculoServico, VeiculoServico>();



            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o Token Jwt aqui"

                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {new  OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                     }
                 }, new string []{ }

                 }
                });
            });

            services.AddDbContext<DbContexto>(options =>
            {
                options.UseMySql(
                   Configuration.GetConnectionString("mysql"),
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("mysql"))
                    );
            });
        }

        #region  App
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                #region  Home

                endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
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
                new( ClaimTypes.Role, administrador.Perfil)


            };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);

                }



                endpoints.MapPost("/login", ([FromBody] LoginDto loginDto, IAdministradorServico administradorServico) =>
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


                }).AllowAnonymous()
                .WithTags("Login")
                .WithName("AuthenticateUser")
                .WithSummary("Endpoint para autenticação de usuários")
                .WithDescription("Recebe credenciais e retorna um token JWT válido");

                endpoints.MapPost("/administrador", ([FromBody] AdministradorDto administradorDto, IAdministradorServico administradorServico) =>
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

                ).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Login");

                endpoints.MapGet("/administradores", ([FromQuery] int? Pagina, IAdministradorServico administradorServico) =>
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


                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Login");


                endpoints.MapGet("/administrador/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
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

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Login");

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

                endpoints.MapPost("/veiculo", ([FromBody] VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
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



                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veiculos");

                endpoints.MapGet("/veiculos", (int? Pagina, IVeiculoServico veiculoServico) =>
                {

                    if (Pagina == null)
                    {
                        Pagina = 1;
                    }

                    var veiculos = veiculoServico.Todos((int)Pagina);
                    return Results.Ok(veiculos);

                }).RequireAuthorization().WithTags("Veiculos");

                endpoints.MapGet("/veiculo/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
                {
                    var veiculo = veiculoServico.BuscarPorId(id);

                    if (veiculo == null) return Results.NotFound();

                    return Results.Ok(veiculo);

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veiculos");


                endpoints.MapPut("/veiculo/{id}", ([FromRoute] int id, VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
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

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veiculos");




                endpoints.MapDelete("/veiculo/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
                {
                    var veiculo = veiculoServico.BuscarPorId(id);

                    if (veiculo == null) return Results.NotFound();

                    veiculoServico.Apagar(veiculo);

                    return Results.NoContent();

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veiculos");

                #endregion
            });

        }
        #endregion
    }


}