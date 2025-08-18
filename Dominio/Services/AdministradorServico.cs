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


        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;


        }
        private readonly DbContexto _contexto;

        public Administrador Login(LoginDto loginDto)
        {
            var administradorLogado = _contexto.Administradores.Where(a => a.Email == loginDto.Email && a.Senha == loginDto.Password).FirstOrDefault();
            return administradorLogado;
        }

        public void Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();

        }



        public List<Administrador> Todos(int pagina = 1)
        {
            int itensPorPagina = 10;
            int pular = (pagina - 1) * itensPorPagina;

            var query = _contexto.Administradores.AsQueryable();

            return query.OrderBy(v => v.Id)
                      .Skip(pular)
                      .Take(itensPorPagina)
                      .ToList();
        }

        public Administrador BuscarPorId(int id)
        {
            return _contexto.Administradores.Find(id);
        }
    }
}