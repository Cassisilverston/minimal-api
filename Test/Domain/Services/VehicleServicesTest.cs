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
    public class VehicleServicesTest
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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
        
        [TestMethod]
        public void TestSaveVehicle()
        {
            var context = CreateContextTest();

            var vehicle = new Vehicle();
            vehicle.Name = "Corolla";
            vehicle.Mark = "Toyota";
            vehicle.Year = 2022;

            var vehicleService = new VehicleService(context);

            vehicleService.Create(vehicle); 

            Assert.AreEqual(1, vehicleService.All(1).Count());
        }
    

        [TestMethod]
        public void TestSearchById()
        {
            var context = CreateContextTest();
            

            var vehicle = new Vehicle();
            vehicle.Name = "Civic";
            vehicle.Mark = "Honda";
            vehicle.Year = 2020;

            context.Vehicles.Add(vehicle);
            context.SaveChanges();

            var vehicleService = new VehicleService(context);

            var result = vehicleService.SearchById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Civic", result.Name);
            Assert.AreEqual("Honda", result.Mark);
        }

        [TestMethod]
        public void TestAll()
        {
            var context = CreateContextTest();

            context.Vehicles.Add(new Vehicle { Name = "Fusca", Mark = "VW", Year = 1970 });
            context.Vehicles.Add(new Vehicle { Name = "Uno", Mark = "Fiat", Year = 2000 });
            context.SaveChanges();

            var vehicleService = new VehicleService(context);

            var list = vehicleService.All(1);

            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count());
        }
    }
}