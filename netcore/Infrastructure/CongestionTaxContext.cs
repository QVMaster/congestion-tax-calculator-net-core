
using Microsoft.EntityFrameworkCore;
using congestion.calculator.Domain.Entities;

namespace congestion.calculator.Infrastructure;

// EF Core DbContext for the Congestion Tax system
// This is part of Phase 2: preparing data persistence layer using SQLite
public class CongestionTaxContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<TaxRule> TaxRules { get; set; }    
    public DbSet<VehiclePass> VehiclePasses { get; set; }
    public DbSet<DailyCapSetting> DailyCapSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Using local SQLite database for persistence
        optionsBuilder.UseSqlite("Data Source=congestiontax.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasMany(c => c.TaxRules)
                  .WithOne(r => r.City)
                  .HasForeignKey(r => r.CityId);

            entity.HasOne(c => c.DailyCapSetting)
                  .WithOne(cs => cs.City)
                  .HasForeignKey<DailyCapSetting>(cs => cs.CityId);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(v => v.Id);

            entity.Property(v => v.PlateNumber)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(v => v.VehicleType)
                  .IsRequired()
                  .HasConversion<int>();

            entity.HasMany(v => v.Passes)
                  .WithOne(p => p.Vehicle)
                  .HasForeignKey(p => p.VehicleId);
        });


        modelBuilder.Entity<VehiclePass>(entity =>
        {
            entity.HasKey(vp => vp.Id);

            entity.Property(vp => vp.PassTime)
                  .IsRequired();

            entity.HasOne(vp => vp.Vehicle)
                  .WithMany(v => v.Passes)
                  .HasForeignKey(vp => vp.VehicleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(vp => vp.City)
                  .WithMany()
                  .HasForeignKey(vp => vp.CityId)
                  .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<TaxRule>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.StartTime).IsRequired();
            entity.Property(r => r.EndTime).IsRequired();

            entity.Property(r => r.Fee)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.HasOne(r => r.City)
                  .WithMany(c => c.TaxRules)
                  .HasForeignKey(r => r.CityId);
        });


        modelBuilder.Entity<DailyCapSetting>(entity =>
        {
            entity.HasKey(cs => cs.Id);

            entity.Property(cs => cs.DailyCap)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.HasOne(cs => cs.City)
                  .WithOne(c => c.DailyCapSetting)
                  .HasForeignKey<DailyCapSetting>(cs => cs.CityId);
        });


        base.OnModelCreating(modelBuilder);
    }
}
