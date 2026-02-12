using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.ModelViews;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class VehicleRequestTest
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
        public async Task TestCreateVehicle()
        {
            using var client = Setup.http.CreateClient();

            var loginDTO = new LoginDTO { Email = "administrator@teste.com", Password = "123456" };
            var loginResponse = await client.PostAsJsonAsync("/administrators/login", loginDTO);
            var loginResult = await loginResponse.Content.ReadAsStringAsync();
            var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(loginResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            #pragma warning disable CS8602
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogged.Token);
            #pragma warning restore CS8602

            var vehicleDTO = new VehicleDTO
            {
                Name = "Corolla",
                Mark = "Toyota",
                Year = 2024
            };

            var response = await client.PostAsJsonAsync("/vehicles", vehicleDTO);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var vehicleCreated = JsonSerializer.Deserialize<Vehicle>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(vehicleCreated);
            Assert.AreEqual("Corolla", vehicleCreated.Name);
            Assert.IsGreaterThan(0, vehicleCreated.Id);
        }

        [TestMethod]
        public async Task TestGetVehicleById()
        {
            using var client = Setup.http.CreateClient();

            var loginDTO = new LoginDTO { Email = "administrator@teste.com", Password = "123456" };
            var loginResponse = await client.PostAsJsonAsync("/administrators/login", loginDTO);
            var loginResult = await loginResponse.Content.ReadAsStringAsync();
            var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(loginResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            #pragma warning disable CS8602
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogged.Token);
            #pragma warning restore CS8602

            var response = await client.GetAsync("/vehicles/1");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var vehicle = JsonSerializer.Deserialize<Vehicle>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(vehicle);
            Assert.AreEqual(1, vehicle.Id);
            Assert.AreEqual("Fusca", vehicle.Name);
        }

        [TestMethod]
        public async Task TestGetAllVehicles()
        {
            using var client = Setup.http.CreateClient();

            var loginDTO = new LoginDTO { Email = "administrator@teste.com", Password = "123456" };
            var loginResponse = await client.PostAsJsonAsync("/administrators/login", loginDTO);
            var loginResult = await loginResponse.Content.ReadAsStringAsync();
            var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(loginResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            #pragma warning disable CS8602
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogged.Token);
            #pragma warning restore CS8602

            var response = await client.GetAsync("/vehicles");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(vehicles);
            Assert.IsNotEmpty(vehicles);
        }

        [TestMethod]
        public async Task TestUpdateVehicle()
        {
            using var client = Setup.http.CreateClient();
            var loginDTO = new LoginDTO { Email = "administrator@teste.com", Password = "123456" };
            var loginResponse = await client.PostAsJsonAsync("/administrators/login", loginDTO);
            var loginResult = await loginResponse.Content.ReadAsStringAsync();
            var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(loginResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            #pragma warning disable CS8602
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogged.Token);
            #pragma warning restore CS8602

            var vehicleDTO = new VehicleDTO { Name = "Fiesta", Mark = "Ford", Year = 2010 };
            var createResponse = await client.PostAsJsonAsync("/vehicles", vehicleDTO);
            var createResult = await createResponse.Content.ReadAsStringAsync();
            var createdVehicle = JsonSerializer.Deserialize<Vehicle>(createResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            #pragma warning disable CS8602
            createdVehicle.Name = "Fiesta Sedan";
            #pragma warning restore CS8602
            createdVehicle.Year = 2011; 

            var response = await client.PutAsJsonAsync($"/vehicles/{createdVehicle.Id}", createdVehicle);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseGet = await client.GetAsync($"/vehicles/{createdVehicle.Id}");
            var resultGet = await responseGet.Content.ReadAsStringAsync();
            var vehicleUpdated = JsonSerializer.Deserialize<Vehicle>(resultGet, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            #pragma warning disable CS8602
            Assert.AreEqual("Fiesta Sedan", vehicleUpdated.Name);
            #pragma warning restore CS8602
            Assert.AreEqual(2011, vehicleUpdated.Year);
        }

        [TestMethod]
        public async Task TestDeleteVehicle()
        {
            using var client = Setup.http.CreateClient();
            var loginDTO = new LoginDTO { Email = "administrator@teste.com", Password = "123456" };
            var loginResponse = await client.PostAsJsonAsync("/administrators/login", loginDTO);
            var loginResult = await loginResponse.Content.ReadAsStringAsync();
            var admLogged = JsonSerializer.Deserialize<AdministratorLogged>(loginResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            #pragma warning disable CS8602
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogged.Token);
            #pragma warning restore CS8602

            var vehicleDTO = new VehicleDTO { Name = "Gol", Mark = "VW", Year = 2022 };
            var createResponse = await client.PostAsJsonAsync("/vehicles", vehicleDTO);
            var createResult = await createResponse.Content.ReadAsStringAsync();
            var createdVehicle = JsonSerializer.Deserialize<Vehicle>(createResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.IsNotNull(createdVehicle);

            var response = await client.DeleteAsync($"/vehicles/{createdVehicle.Id}");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            var responseGet = await client.GetAsync($"/vehicles/{createdVehicle.Id}");
            Assert.AreEqual(HttpStatusCode.NotFound, responseGet.StatusCode);
        }
    }
}