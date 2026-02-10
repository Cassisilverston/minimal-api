using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Domain.Entities;
using MinimalApi.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using System.IO;
using MinimalApi.Domain.Services;
using MinimalApi.Domain.DTOs;

namespace Test.Domain.Entities
{
    [TestClass]
    [DoNotParallelize]
    public class AdministratorServiceTest
    {
        private AppDbContext CreateContextTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<AdministratorServiceTest>();

            var configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("MySql");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;

            return new AppDbContext(options);
        }

        [TestMethod]
        public void TestSaveAdministrator()
        {
            // 1. Arrange (Preparar) 
            var context = CreateContextTest();

            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            var adm = new Administrator();
            adm.Email = "teste@teste.com";
            adm.Password = "teste";
            adm.Profile = "Adm";

            var administratorService = new AdministratorService(context);

            // 2. Act (Agir) 
            context.Administrators.Add(adm);
            context.SaveChanges();

            // 3. Assert (Validar)
            Assert.AreEqual(1, administratorService.All(1).Count());
        }

        [TestMethod]
        public void TestSearchById()
        {
            var context = CreateContextTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            var adm = new Administrator();
            adm.Email = "teste@teste.com";
            adm.Password = "teste";
            adm.Profile = "Adm";

            context.Administrators.Add(adm);
            context.SaveChanges();

            var administratorService = new AdministratorService(context);

            // 2. Act (Ação: Tentar buscar o ID 1)
            var result = administratorService.SearchById(1);

            // 3. Assert (Validar)
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("teste@teste.com", result.Email);
        }

        [TestMethod]
        public void TestLogin()
        {
            // 1. Arrange
            var context = CreateContextTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            var adm = new Administrator();
            adm.Email = "teste@teste.com";
            adm.Password = "teste";
            adm.Profile = "Adm";

            context.Administrators.Add(adm);
            context.SaveChanges();

            var administratorService = new AdministratorService(context);

            var loginDto = new LoginDTO
            {
                Email = "teste@teste.com",
                Password = "teste"
            };

            // 2. Act
            var loginResult = administratorService.Login(loginDto);

            // 3. Assert
            Assert.IsNotNull(loginResult);
            Assert.AreEqual("teste@teste.com", loginResult.Email);
            Assert.AreEqual("Adm", loginResult.Profile);
        }

        [TestMethod]
        public void TestLoginFailure()
        {
            // 1. Arrange 
            var context = CreateContextTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            var adm = new Administrator();
            adm.Email = "teste@teste.com";
            adm.Password = "123456"; // Senha correta no banco
            adm.Profile = "Adm";

            context.Administrators.Add(adm);
            context.SaveChanges();

            var administratorService = new AdministratorService(context);

            var loginDto = new LoginDTO
            {
                Email = "teste@teste.com",
                Password = "senha_errada"
            };

            // 2. Act 
            var loginResult = administratorService.Login(loginDto);

            // 3. Assert
            Assert.IsNull(loginResult);
        }

        [TestMethod]
        public void TestAll()
        {
            // 1. Arrange
            var context = CreateContextTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            // adicionei dois usuários para ter a tal da "certeza", pois estou aprendendo e fixando
            context.Administrators.Add(new Administrator { Email = "teste1@teste.com", Password = "1", Profile = "Adm" });
            context.Administrators.Add(new Administrator { Email = "teste2@teste.com", Password = "2", Profile = "Editor" });
            context.SaveChanges();

            var administratorService = new AdministratorService(context);

            // 2. Act
            var list = administratorService.All(1);

            // 3. Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count());
        }
    }
}