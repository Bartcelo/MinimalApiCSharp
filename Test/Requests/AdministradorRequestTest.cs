
using System.Text;
using System.Text.Json;
using Minimal_Api.Dominio.DTOs;
using Minimal_Api.Dominio.ModelViews;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class AdministradorRequestTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }


        [TestMethod]
        public async Task TestarGetSetPropriedades()
        {

            // Arrange

            var loginDto = new LoginDto
            {
                Email = "administrador@test.com",
                Password = "123456"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "Application/Json");


            // Act


            var response = await Setup.client.PostAsync("/login ", content);

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);

            var result = await response.Content.ReadAsByteArrayAsync();
            var admlogado = JsonSerializer.Deserialize<AdiministradorLogado>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true

            });

            Assert.IsNotNull(admlogado?.Email  ??  "");
            Assert.IsNotNull(admlogado?.Perfil ??  "");
            Assert.IsNotNull(admlogado?.Token  ??  "");


        }
    }
}