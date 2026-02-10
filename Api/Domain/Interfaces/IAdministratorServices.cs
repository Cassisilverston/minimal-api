using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces
{
    public interface IAdministratorServices
    {
     Administrator? Login(LoginDTO loginDTO);

     Administrator Add(Administrator administrator);  
     
     Administrator? SearchById(int id);

     List<Administrator> All(int? page); 

    }
}