using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal_Api.Dominio.Entidades;

namespace Test.Domain
{
    [TestClass]
    public class VeiculosTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var veiculo = new Veiculo();

            veiculo.Id = 2;
            veiculo.Nome = "Logan";
            veiculo.Marca = "Renalt";
            veiculo.Ano = 2010;



            Assert.AreEqual(2, veiculo.Id);
            Assert.AreEqual("Logan", veiculo.Nome);
            Assert.AreEqual("Renalt", veiculo.Marca);
            Assert.AreEqual(2010, veiculo.Ano);



        }
    }
}