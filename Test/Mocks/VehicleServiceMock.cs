using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Services;

namespace Test.Mocks
{
    public class VehicleServiceMock : IVehicleServices
    {
        private static List<Vehicle> vehicles = new List<Vehicle>()
        {
            new Vehicle { Id = 1, Name = "Fusca", Mark = "VW", Year = 1970 }
        };

        public void Create(Vehicle vehicle)
        {
            vehicle.Id = vehicles.Count + 1;
            vehicles.Add(vehicle);
        }

        public void DeleteByVehicle(Vehicle vehicle)
        {
            vehicles.Remove(vehicle);
        }

        public List<Vehicle> All(int? page = 1, string? name = null, string? mark = null)
        {
            return vehicles;
        }

        public Vehicle? SearchById(int id)
        {
            return vehicles.Find(v => v.Id == id);
        }

        public void Update(Vehicle vehicle)
        {
            var index = vehicles.FindIndex(v => v.Id == vehicle.Id);
            if (index != -1)
            {
                vehicles[index] = vehicle;
            }
        }
    }
}