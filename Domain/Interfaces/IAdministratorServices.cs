using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces
{
    public interface IAdministratorServices
    {
     Administrator? Login(LoginDTO loginDTO);   
    }
}