using Microsoft.EntityFrameworkCore;
using RealEstateCRMWinForms.Models;

namespace RealEstateCRMWinForms.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyProofFile> PropertyProofFiles { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Inquiry> Inquiries { get; set; }
        public DbSet<DealStatusChangeRequest> DealStatusChangeRequests { get; set; }

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

            // Configure Inquiry relationships
            modelBuilder.Entity<Inquiry>()
                .HasOne(i => i.Property)
                .WithMany()
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inquiry>()
                .HasOne(i => i.Client)
                .WithMany()
                .HasForeignKey(i => i.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Inquiry>()
                .HasOne(i => i.RespondedByBroker)
                .WithMany()
                .HasForeignKey(i => i.RespondedByBrokerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure DealStatusChangeRequest relationships
            modelBuilder.Entity<DealStatusChangeRequest>()
                .HasOne(dscr => dscr.Deal)
                .WithMany()
                .HasForeignKey(dscr => dscr.DealId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DealStatusChangeRequest>()
                .HasOne(dscr => dscr.RequestedBy)
                .WithMany()
                .HasForeignKey(dscr => dscr.RequestedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DealStatusChangeRequest>()
                .HasOne(dscr => dscr.RespondedBy)
                .WithMany()
                .HasForeignKey(dscr => dscr.RespondedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
