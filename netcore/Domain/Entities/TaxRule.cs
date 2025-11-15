// Represents time-based tax rules for each city

using System;

namespace congestion.calculator.Domain.Entities;

public class TaxRule
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public decimal Fee { get; set; }

    public City City { get; set; }
}
