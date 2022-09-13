using CarRental.Data.Entities;
using CarRental.Data.Utils;
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
            modelBuilder.Entity<VehicleEntity>()
                .Property(x => x.PricePerDay)
                .HasPrecision(10,2);

            modelBuilder.Entity<ClientEntity>()
                .HasIndex(c => new { c.Email, c.Active })
                .IsUnique();

            modelBuilder.Entity<VehicleEntity>()
                .HasIndex(v => new { v.Model, v.Active})
                .IsUnique();

            modelBuilder.Entity<RentalEntity>(builder =>
            {
                builder.Property(x => x.DateFrom)
                    .HasConversion<DateOnlyConverter, DateOnlyComparer>();

                builder.Property(x => x.DateTo)
                    .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            });

            modelBuilder.Entity<RentalEntity>()
                .Property(x => x.Price)
                .HasPrecision(10, 2);


        }
    }
}
