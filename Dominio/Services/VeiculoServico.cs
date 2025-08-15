using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Minimal_Api.Dominio.Entidades;
using Minimal_Api.Dominio.Interfaces;
using Minimal_Api.Infraestrutura.Db;

namespace Minimal_Api.Dominio.Services
{
    public class VeiculoServico : IVeiculoServico
    {

        private readonly DbContexto _contexto;

        public VeiculoServico(DbContexto contexto)
        {
            _contexto = contexto;


        }
        public void Apagar(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public void Atualizar(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }

        public Veiculo BuscarPorId(int id)
        {
            return _contexto.Veiculos.Find(id);
        }

        public void Incluir(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }

        public List<Veiculo> Todos(int pagina = 1, string nome = null, string marca = null)
        {
           
            int itensPorPagina = 10;
            int pular = (pagina - 1) * itensPorPagina;

           
            var query = _contexto.Veiculos.AsQueryable();


            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => v.Nome.Contains(nome));
                query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(),$"%{nome}%"));
            }

            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(v => v.Marca.Contains(marca));
            }

            
            return query.OrderBy(v => v.Nome)
                       .Skip(pular)
                       .Take(itensPorPagina)
                       .ToList();

        }
    }
}