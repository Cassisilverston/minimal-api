using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.ModelViews;
using Test.Helpers;
using System.Net.Http.Headers;

namespace Test.Requests
{
    [TestClass]
    public class AdministratorRequestTest
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
        public async Task TestLogin()
        {
            // 1. Arrange
            var loginDTO = new LoginDTO
            {
                Email = "administrator@teste.com",
                Password = "123456"
            };

            // 2. Act
            var response = await Setup.client.PostAsJsonAsync("/administrators/login", loginDTO);

            // 3. Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);


            var result = await response.Content.ReadAsStringAsync();

            var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Ignora maiúsculas/minúsculas
            });

            Assert.IsNotNull(admLogged);
            Assert.IsNotNull(admLogged.Email);
            Assert.IsNotNull(admLogged.Profile);
            Assert.IsNotNull(admLogged.Token);
        }

        [TestMethod]
        public async Task TestCreateAdministrator()
        {
            using var client = Setup.http.CreateClient();

            var loginDTO = new LoginDTO
            {
                Email = "administrator@teste.com",
                Password = "123456"
            };

            var loginResponse = await client.PostAsJsonAsync("/administrators/login", loginDTO);

            var loginResult = await loginResponse.Content.ReadAsStringAsync();
            var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(loginResult, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admLogged, "O Login retornou nulo!");
            Assert.IsNotNull(admLogged.Token, "O Token veio vazio!");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogged.Token);

            var admDTO = new AdministratorDTO
            {
                Email = "novo_adm@teste.com",
                Password = "123456",
                Profile = Profile.Editor
            };

            var response = await client.PostAsJsonAsync("/administrators", admDTO);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var admCreated = JsonSerializer.Deserialize<Administrator>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admCreated);
            Assert.AreEqual("novo_adm@teste.com", admCreated.Email);
            Assert.IsGreaterThan(0, admCreated.Id);
        }

        [TestMethod]
        public async Task TestGetById()
        {
            using var client = Setup.http.CreateClient();

            var loginDTO = new LoginDTO { Email = "administrator@teste.com", Password = "123456" };
            var loginResponse = await client.PostAsJsonAsync("/administrators/login", loginDTO);
            var loginResult = await loginResponse.Content.ReadAsStringAsync();
            var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(loginResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            #pragma warning disable CS8602
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogged.Token);
            #pragma warning restore CS8602

            var response = await client.GetAsync("/administrators/1");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var adm = JsonSerializer.Deserialize<AdministratorModelView>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(adm);
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("administrator@teste.com", adm.Email);
        }

        [TestMethod]
        public async Task TestGetAll()
        {
            using var client = Setup.http.CreateClient();

            var loginDTO = new LoginDTO { Email = "administrator@teste.com", Password = "123456" };
            var loginResponse = await client.PostAsJsonAsync("/administrators/login", loginDTO);

            var loginResult = await loginResponse.Content.ReadAsStringAsync();
            var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(loginResult, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admLogged);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogged.Token);

            var response = await client.GetAsync("/administrators");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var adms = JsonSerializer.Deserialize<List<Administrator>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(adms);
            Assert.AreNotEqual(0, adms.Count);
        }
    }
}