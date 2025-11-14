// Represents a vehicle's passage record (for future use in reporting)

using System;

namespace congestion.calculator.Domain.Entities;

public class VehiclePass
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public int VehicleId { get; set; }
    public DateTime PassTime { get; set; }

    public City City { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
}
