using CarTrade.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarTrade.Data
{
    public class CarDbContext : IdentityDbContext<User>
    {
        //when manual initial migration
        //public CarDbContext()
        //{

        //}

        public CarDbContext(DbContextOptions<CarDbContext> options)
           : base(options)
        {
        }

        //when manual initial migration
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // Specify that we will use sqlite and the path of the database here
        //    var options = optionsBuilder
        //        .UseSqlServer("ConnectionString");

        //    base.OnConfiguring(options);
        //}

        public DbSet<VehicleStuff> VehicleStuffs { get; set; }

        public DbSet<VehiclePicture> VehiclePictures { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Branch> Branches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Rental>()
                .HasOne(v => v.Vehicle)
                .WithMany(r => r.Rentals)
                .HasForeignKey(v => v.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            //    builder.Entity<Rental>()
            //       .HasOne(c => c.Driver)
            //       .WithMany()
            //       .HasForeignKey(v => v.UserId);

            base.OnModelCreating(builder);
        }
    }
}
