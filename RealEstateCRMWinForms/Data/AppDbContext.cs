using Microsoft.EntityFrameworkCore;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<Board> Boards { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Deal relationships
            modelBuilder.Entity<Deal>()
                .HasOne(d => d.Property)
                .WithMany()
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Deal>()
                .HasOne(d => d.Contact)
                .WithMany()
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Board
            modelBuilder.Entity<Board>()
                .HasIndex(b => b.Name)
                .IsUnique();
        }
    }
}
