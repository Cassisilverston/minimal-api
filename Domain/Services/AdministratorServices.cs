using Microsoft.AspNetCore.Identity.Data;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Services
{
    public class AdministratorService : IAdministratorServices
    {
        private readonly AppDbContext _contexto;
        public AdministratorService(AppDbContext context)
        {
         _contexto = context;
        }
        public Administrator? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return adm;
        }
    }
}