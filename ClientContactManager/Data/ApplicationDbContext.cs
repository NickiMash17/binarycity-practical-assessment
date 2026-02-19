using Microsoft.EntityFrameworkCore;
using ClientContactManager.Models;

namespace ClientContactManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<ClientContact> ClientContacts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship with composite key
            modelBuilder.Entity<ClientContact>()
                .HasKey(cc => new { cc.ClientId, cc.ContactId });

            modelBuilder.Entity<ClientContact>()
                .HasOne(cc => cc.Client)
                .WithMany(c => c.ClientContacts)
                .HasForeignKey(cc => cc.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientContact>()
                .HasOne(cc => cc.Contact)
                .WithMany(c => c.ClientContacts)
                .HasForeignKey(cc => cc.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure email uniqueness
            modelBuilder.Entity<Contact>()
                .Property(c => c.Email)
                .UseCollation("NOCASE");

            modelBuilder.Entity<Contact>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}
