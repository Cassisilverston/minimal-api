using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces
{
    public interface IVehicleServices
    {
     List<Vehicle> All(int? page = 1, string? name = null, string? mark = null);

     Vehicle? SearchById(int id);

     void Inside(Vehicle vehicle); 

     void ToUpdate(Vehicle vehicle);

     void DeleteByVehicle(Vehicle vehicle);
    }
}