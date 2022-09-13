using CarRental.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data
{
    public class CarRentalContext : DbContext
    {
        public CarRentalContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public virtual DbSet<VehicleEntity> Vehicles { get; set; }
        public virtual DbSet<ClientEntity> Clients { get; set; }
        public virtual DbSet<RentalEntity> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientEntity>()
                .HasIndex(c => new { c.Email, c.Active })
                .IsUnique();

            modelBuilder.Entity<VehicleEntity>()
                .HasIndex(v => new { v.Model, v.Active})
                .IsUnique();
        }
    }
}
