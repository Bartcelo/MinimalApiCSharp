using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minimal_Api.Dominio.DTOs;
using Minimal_Api.Dominio.Entidades;
using Minimal_Api.Dominio.Interfaces;

namespace Test.Mocks
{
    public class AdministradorServicoMock : IAdministradorServico
    {

        private static List<Administrador> administradoresmock = new List<Administrador>()
        {
            new Administrador{
                Id = 1,
                Email = "administrador@test.com",
                Senha = "123456",
                Perfil = "Adm"
             },
             new Administrador{
                Id = 2,
                Email = "editor@test.com",
                Senha = "123456",
                Perfil = "Editor"
             }
        };

        public Administrador BuscarPorId(int id)
        {
            return administradoresmock.Find(a => a.Id == id);
        }

        public void Incluir(Administrador administrador)
        {
            administrador.Id = administradoresmock.Count() + 1;
            administradoresmock.Add(administrador);

            
        }

        public Administrador Login(LoginDto loginDto)
        {
            return administradoresmock.Find(a => a.Email == loginDto.Email && a.Senha == loginDto.Password );
        }

        public List<Administrador> Todos(int pagina = 1)
        {
            return administradoresmock;
        }
    }
}