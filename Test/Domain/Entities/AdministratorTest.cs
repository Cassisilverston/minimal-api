using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class AdministratorTest
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            // 1. Arrange (Preparar) - criação do objeto e definição do cenário
            var adm = new Administrator();

            // 2. Act (Agir) - executa a ação que queremos testar
            adm.Id = 1;
            adm.Email = "teste@teste.com";
            adm.Password = "teste";
            adm.Profile = "Adm";

            // 3. Assert (Validar) - verifica se o resultado é o esperado
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("teste@teste.com", adm.Email);
            Assert.AreEqual("teste", adm.Password);
            Assert.AreEqual("Adm", adm.Profile);
        }
    }
}