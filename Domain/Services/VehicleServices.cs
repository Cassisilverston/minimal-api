using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Services
{
    public class VehicleService : IVehicleServices
    {
        private readonly AppDbContext _contexto;
        public VehicleService(AppDbContext context)
        {
         _contexto = context;
        }

        public List<Vehicle> All(int page = 1, string? name = null, string? mark = null)
        {
            var query = _contexto.Vehicles.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name}%"));
            }

            int itemsPorPage = 10;

            query = query.Skip((page - 1) * itemsPorPage).Take(itemsPorPage);

            return query.ToList();
        }

        public void DeleteByVehicle(Vehicle vehicle)
        {
            _contexto.Vehicles.Remove(vehicle);
            _contexto.SaveChanges();
        }

        public void Inside(Vehicle vehicle)
        {
            _contexto.Vehicles.Add(vehicle);
            _contexto.SaveChanges();
        }

        public Vehicle? SearchById(int id)
        {
            return _contexto.Vehicles.Where(v => v.Id == id).FirstOrDefault();
        }

        public void ToUpdate(Vehicle vehicle)
        {
            _contexto.Vehicles.Update(vehicle);
            _contexto.SaveChanges();
        }
    }
}