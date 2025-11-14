// Represents daily cap setting (maximum tax per day for a city)


// Represents daily cap setting (maximum tax per day for a city)

namespace congestion.calculator.Domain.Entities;

public class DailyCapSetting
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public decimal DailyCap { get; set; }

    public City City { get; set; } = null!;
}
