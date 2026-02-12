using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.DTOs;


namespace Test.Mocks
{
    public class AdministratorServiceMock : IAdministratorServices
    {
        private static List<Administrator> administrators = new List<Administrator>()
        {
            new Administrator { Id = 1, Email = "administrator@teste.com", Password = "123456", Profile = "Adm" },
            new Administrator { Id = 2, Email = "editor@teste.com", Password = "123456", Profile = "Editor"}
        };

        public Administrator? SearchById(int id)
        {
            return administrators.Find(a => a.Id == id);
        }


        public Administrator? Login(LoginDTO loginDTO)
        {
            return administrators.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
        }

        public Administrator Add(Administrator administrator)
        {
            administrator.Id = administrators.Count() + 1;
            administrators.Add(administrator);
            
            return administrator;
        }

        public List<Administrator> All(int? page)
        {
            return administrators;
        }
    }
}