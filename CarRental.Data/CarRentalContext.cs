using CarRental.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data
{
    public class CarRentalContext : DbContext
    {
        public CarRentalContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<VehicleEntity> Vehicles { get; set; }
        public DbSet<ClientEntity> Clients { get; set; }
        public DbSet<RentalEntity> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientEntity>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<VehicleEntity>()
                .HasIndex(u => u.Model)
                .IsUnique();
        }
    }
}
