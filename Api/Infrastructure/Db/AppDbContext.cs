using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace MinimalApi.Infrastructure.Db
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration? _configurationAppSettings;

         public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public AppDbContext(IConfiguration configurationAppSettings)
        {
            _configurationAppSettings = configurationAppSettings;
        }
        public DbSet<Administrator> Administrators { get; set; } = default!;

        public DbSet<Vehicle> Vehicles { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>().HasData(
                new Administrator
                {
                    Id = 1,
                    Email = "administrator@teste.com",
                    Password = "123456",
                    Profile = "Adm"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                var stringConnection = _configurationAppSettings.GetConnectionString("MySql")?.ToString();
                if (!string.IsNullOrEmpty(stringConnection))
                {
                    optionsBuilder.UseMySql(stringConnection, ServerVersion.AutoDetect(stringConnection));
                }
            }
        }
    }
}