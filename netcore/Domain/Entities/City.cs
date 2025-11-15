// Represents a city with specific congestion tax rules

using System.Collections.Generic;

namespace congestion.calculator.Domain.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<TaxRule> TaxRules { get; set; } = new List<TaxRule>();
    public DailyCapSetting DailyCapSetting { get; set; } = default!;
}
