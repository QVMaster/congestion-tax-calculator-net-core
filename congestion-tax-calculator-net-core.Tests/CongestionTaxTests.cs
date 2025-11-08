using Xunit;
using congestion.calculator;

namespace congestion_tax_calculator_net_core.Tests;

public class CongestionTaxTests
{
    private readonly CongestionTaxCalculator _congestionTaxCalculator = new();
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
        Assert.True(newCalcTax > singleDayCalcTax, $"newCalcTax: {newCalcTax} | singleDayCalcTax: {singleDayCalcTax} -> New version should fix undercounting across multiple days");
    }


    // Test that a single vehicle pass at a specific time returns the correct toll fee    
    [Fact]
    public void SinglePass_ShouldReturnCorrectFee()
    {
        var vehicle = new MockVehicle(VehicleType.Car);
        var dates = new DateTime[] { new DateTime(2013, 2, 7, 7, 10, 0) };
        int fee = _congestionTaxCalculator.GetTax(vehicle, dates);
        Assert.Equal(18, fee);
    }

    // Test that multiple passes within a 60-minute window apply only the highest fee    
    [Fact]
    public void MultiplePassesWithin60Minutes_ShouldApplySingleChargeRule()
    {
        var vehicle = new MockVehicle(VehicleType.Car);
        var dates = new DateTime[]
        {
            new DateTime(2013, 2, 7, 6, 50, 0),
            new DateTime(2013, 2, 7, 7, 30, 0)
        };
        int fee = _congestionTaxCalculator.GetTax(vehicle, dates);
        Assert.Equal(18, fee);
    }

    // Test that passes separated by more than 60 minutes are counted in separate windows
    [Fact]
    public void PassesOver60Minutes_ShouldSumFeesOfTwoWindows()
    {
        var vehicle = new MockVehicle(VehicleType.Car);
        var dates = new DateTime[]
        {
        new DateTime(2013, 2, 7, 6, 0, 0),
        new DateTime(2013, 2, 7, 7, 10, 0)
        };
        int fee = _congestionTaxCalculator.GetTax(vehicle, dates);
        Assert.Equal(26, fee); // sum of two separate windows
    }

    // Test that exempt vehicles return zero fee regardless of pass times
    [Fact]
    public void TollFreeVehicle_ShouldReturnZero()
    {
        var vehicle = new MockVehicle(VehicleType.Motorcycle);
        var dates = new DateTime[]
        {
        new DateTime(2013, 2, 7, 7, 0, 0),
        new DateTime(2013, 2, 7, 8, 0, 0)
        };
        int fee = _congestionTaxCalculator.GetTax(vehicle, dates);
        Assert.Equal(0, fee);
    }

    // Test that passes on holidays or weekends return zero fee
    [Fact]
    public void TollFreeDate_ShouldReturnZero()
    {
        var vehicle = new MockVehicle(VehicleType.Car);
        var dates = new DateTime[] { new DateTime(2013, 12, 25, 10, 0, 0) }; // Christmas
        int fee = _congestionTaxCalculator.GetTax(vehicle, dates);
        Assert.Equal(0, fee);
    }

    // Test that the known list of post-it dates produces the expected total fee
    [Fact]
    public void PostItList_ShouldReturnExpectedTotal()
    {
        var vehicle = new MockVehicle(VehicleType.Car);
        var dates = new DateTime[]
        {
        new DateTime(2013, 1, 14, 21, 0, 0),  // SEK 0
        new DateTime(2013, 1, 15, 21, 0, 0),  // SEK 0
        new DateTime(2013, 2, 7, 6, 23, 27),  // SEK 8
        new DateTime(2013, 2, 7, 15, 27, 0),  // SEK 13
        new DateTime(2013, 2, 8, 6, 20, 27),  // SEK 8  Removed by a single charge rule applies
        new DateTime(2013, 2, 8, 6, 27, 0),   // SEK 8  
        new DateTime(2013, 2, 8, 14, 35, 0),  // SEK 8  Removed by a single charge rule applies
        new DateTime(2013, 2, 8, 15, 29, 0),  // SEK 13
        new DateTime(2013, 2, 8, 15, 47, 0),  // SEK 18 Removed by a single charge rule applies
        new DateTime(2013, 2, 8, 16, 1, 0),   // SEK 18
        new DateTime(2013, 2, 8, 16, 48, 0),  // SEK 18
        new DateTime(2013, 2, 8, 17, 49, 0),  // SEK 13
        new DateTime(2013, 2, 8, 18, 29, 0),  // SEK 8  Removed by a single charge rule applies
        new DateTime(2013, 2, 8, 18, 35, 0),  // SEK 0  Rule(The maximum amount per day and vehicle is 60 SEK.) applied for 2013-02-08  
        new DateTime(2013, 3, 26, 14, 25, 0), // SEK 8
        new DateTime(2013, 3, 28, 14, 7, 27)  // SEK 0 Days before a public holiday
        };
        int expectedFee = 89; // Total tax based on the post-it list provided in the assignment
        int fee = _congestionTaxCalculator.GetTax(vehicle, dates);
        Assert.Equal(expectedFee, fee);
    }
}

internal class MockVehicle : IVehicle
{
    private readonly VehicleType _type;
    public MockVehicle(VehicleType type) => _type = type;
    public VehicleType Type => _type;
}
