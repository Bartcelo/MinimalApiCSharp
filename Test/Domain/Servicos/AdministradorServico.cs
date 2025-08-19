using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Minimal_Api.Dominio.Entidades;
using Minimal_Api.Dominio.Services;
using Minimal_Api.Infraestrutura.Db;


namespace Test.Domain.Servicos
{
    [TestClass]
    public class AdministradorServicoTest
    {
        private DbContexto CriarContextoDeTeste()
        {

            var assemblypath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblypath ?? "", "..", "..", ".."));


            var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
            var configuration = builder.Build();

            return new DbContexto(configuration);

        }



        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");

            var administrador = new Administrador
            {
                Email = "administrador01@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            };

            var administradorServico = new AdministradorServico(context);


            administradorServico.Incluir(administrador);

            Assert.AreEqual(1, administradorServico.Todos(1).Count());
            // Assert.AreEqual("administrador@teste.com", administrador.Email);
            // Assert.AreEqual("123456", administrador.Senha);
            // Assert.AreEqual("Adm", administrador.Perfil);

        }

         [TestMethod]
        public void TestandoBuscarPorId()
        {
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");

            var administrador = new Administrador
            {
                Email = "administrador02@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            };

            var administradorServico = new AdministradorServico(context);

            administradorServico.Incluir(administrador);
           var admdobanco =  administradorServico.BuscarPorId(administrador.Id);

            Assert.AreEqual(1, admdobanco.Id);
           

        }
    }
}