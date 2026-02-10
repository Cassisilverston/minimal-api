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

           public Administrator? SearchById(int id)
        {
            return _contexto.Administrators.Where(v => v.Id == id).FirstOrDefault();
        }

        public Administrator Add(Administrator administrator)
        {
            _contexto.Administrators.Add(administrator);
            _contexto.SaveChanges();

            return administrator;
        }
        public Administrator? Login(AdministratorDTO loginDTO)
        {
            var adm = _contexto.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return adm;
        }

        public List<Administrator> All(int? page)
        {
            var query = _contexto.Administrators.AsQueryable();
          
            int itemsPorPage = 10;

            if (page != null)
            {
                query = query.Skip(((int)page - 1) * itemsPorPage).Take(itemsPorPage);
            }
            

            return query.ToList();
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            return _contexto.Administrators.FirstOrDefault(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
        }
    }
}