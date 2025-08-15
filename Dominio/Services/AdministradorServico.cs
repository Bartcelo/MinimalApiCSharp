using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Minimal_Api.Dominio.DTOs;
using Minimal_Api.Dominio.Entidades;
using Minimal_Api.Dominio.Interfaces;
using Minimal_Api.Infraestrutura.Db;

namespace Minimal_Api.Dominio.Services
{
    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _contexto;

        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;


        }
        public Administrador Login(LoginDto loginDto)
        {
            var administradorLogado = _contexto.Administradores.Where(a => a.Email == loginDto.Email && a.Senha == loginDto.Password).FirstOrDefault();
            return administradorLogado;
        }
    }
}