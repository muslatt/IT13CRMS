using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RealEstateCRMWinForms.Data
{
    public static class DbContextHelper
    {
        public static AppDbContext CreateDbContext()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("AzureDb");

            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
