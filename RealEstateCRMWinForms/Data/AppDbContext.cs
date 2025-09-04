using Microsoft.EntityFrameworkCore;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
