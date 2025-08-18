using Minimal_Api.Dominio.Entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class AdministradorTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {

            var administrador = new Administrador();

            administrador.Id = 1;
            administrador.Email = "administrador@teste.com";
            administrador.Senha = "123456";
            administrador.Perfil = "Adm";



            Assert.AreEqual(1,administrador.Id);
            Assert.AreEqual("administrador@teste.com", administrador.Email);
            Assert.AreEqual("123456",administrador.Senha);
            Assert.AreEqual("Adm",administrador.Perfil);

        }
    }
}