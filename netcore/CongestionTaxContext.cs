
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace congestion.calculator;

// EF Core DbContext for the Congestion Tax system
// This is part of Phase 2: preparing data persistence layer using SQLite
public class CongestionTaxContext : DbContext
{
    public DbSet<City> Cities { get; set; }
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
        // Fluent configuration can be added here later
        modelBuilder.Entity<City>().HasMany(c => c.TaxRules).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}

// Represents a city with specific congestion tax rules
public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<TaxRule> TaxRules { get; set; } = new List<TaxRule>();
}

// Represents time-based tax rules for each city
public class TaxRule
{
    public int Id { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Fee { get; set; }
}

// Represents a vehicle's passage record (for future use in reporting)
public class VehiclePass
{
    public int Id { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public DateTime PassTime { get; set; }
    public string CityName { get; set; } = string.Empty;
}

// Represents daily cap setting (maximum tax per day for a city)
public class DailyCapSetting
{
    public int Id { get; set; }
    public string CityName { get; set; } = string.Empty;
    public int DailyCap { get; set; }
}
