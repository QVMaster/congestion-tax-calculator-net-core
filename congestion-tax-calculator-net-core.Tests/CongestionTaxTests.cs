using Xunit;
using congestion.calculator;

namespace congestion_tax_calculator_net_core.Tests;

public class CongestionTaxTests
{
    private readonly CongestionTaxCalculator _congestionTaxCalculator;
    [Fact]
    public void GetTax_ShouldReturnIncorrectResult_WhenNotGroupedByDate()
    {
        // Arrange
        var vehicle = new MockVehicle(VehicleType.Car);
        var dates = new[]
        {
            //new DateTime(2013, 2, 7, 6, 23, 0),
            //new DateTime(2013, 2, 7, 15, 27, 0),
            //new DateTime(2013, 2, 8, 6, 27, 0),
            //new DateTime(2013, 2, 8, 16, 1, 0)

            new DateTime(2013, 1, 14, 6, 15, 0),  // ساعت 6:15 - 8 SEK
            new DateTime(2013, 1, 14, 7, 30, 0),  // ساعت 7:30 - 18 SEK
            new DateTime(2013, 1, 14, 15, 5, 0),  // ساعت 15:05 - 13 SEK
            new DateTime(2013, 1, 14, 18, 20, 0),  // ساعت 18:20 - 8 SEK

            new DateTime(2013, 1, 15, 15, 5, 0),  // ساعت 15:05 - 13 SEK
            new DateTime(2013, 1, 15, 18, 20, 0)  // ساعت 18:20 - 8 SEK
        };

        // Act
        var singleDayCalcTax = _congestionTaxCalculator.GetTaxForSingleDay(vehicle, dates);
        var newCalcTax = _congestionTaxCalculator.GetTax(vehicle, dates);

        // Assert
        Assert.NotEqual(singleDayCalcTax, newCalcTax);
        //Assert.True(newCalcTax > singleDayCalcTax, $"newCalcTax: {newCalcTax} | singleDayCalcTax: {singleDayCalcTax} -> New version should fix undercounting across multiple days");
    }
}

internal class MockVehicle : IVehicle
{
    private readonly VehicleType _type;
    public MockVehicle(VehicleType type) => _type = type;
    public VehicleType Type => _type;
}
