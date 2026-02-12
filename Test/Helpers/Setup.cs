using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi;
using MinimalApi.Domain.Interfaces;
using Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Test.Helpers
{
    public class Setup
    {
        public const string PORT = "5001";

        public static TestContext testContext = default!;
        public static WebApplicationFactory<Startup> http = default!;
        public static HttpClient client = default!;

        public static void ClassInit(TestContext testContext)
        {
            #pragma warning disable MSTEST0024
            Setup.testContext = testContext;
            #pragma warning restore MSTEST0024
            Setup.http = new WebApplicationFactory<Startup>();

            Setup.http = Setup.http.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAdministratorServices));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddScoped<IAdministratorServices, AdministratorServiceMock>();

                    var descriptorVehicle = services.SingleOrDefault(d => d.ServiceType == typeof(IVehicleServices));
                    if (descriptorVehicle != null)
                        services.Remove(descriptorVehicle);

                    services.AddScoped<IVehicleServices, VehicleServiceMock>();
                });
            });

            Setup.client = Setup.http.CreateClient();
        }

        public static void ClassCleanup()
        {
            Setup.http.Dispose();
        }
    }
}