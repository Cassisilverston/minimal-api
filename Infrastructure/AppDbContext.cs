using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infrastructure.Db
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configurationAppSettings;

        public AppDbContext(IConfiguration configurationAppSettings)
        {
            _configurationAppSettings = configurationAppSettings;
        }
        public DbSet<Administrator> Administrators { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                var stringConnection = _configurationAppSettings.GetConnectionString("mysql")?.ToString();
                if (!string.IsNullOrEmpty(stringConnection))
                {
                    optionsBuilder.UseMySql(stringConnection, ServerVersion.AutoDetect(stringConnection));
                }
            }
        }
    }
}