using congestion.calculator;

var calculator = new CongestionTaxCalculator();
IVehicle car = new Car();

// چند زمان مختلف برای تست
DateTime[] dates =
{
            new DateTime(2013, 1, 14, 6, 15, 0),  // ساعت 6:15 - 8 SEK
            new DateTime(2013, 1, 14, 7, 30, 0),  // ساعت 7:30 - 18 SEK
            new DateTime(2013, 1, 14, 15, 5, 0),  // ساعت 15:05 - 13 SEK
            new DateTime(2013, 1, 14, 18, 20, 0),  // ساعت 18:20 - 8 SEK

            new DateTime(2013, 1, 15, 15, 5, 0),  // ساعت 15:05 - 13 SEK
            new DateTime(2013, 1, 15, 18, 20, 0)  // ساعت 18:20 - 8 SEK
        };

int tax = calculator.GetTax(car, dates);

Console.WriteLine($"Total tax for the days: {tax} SEK");
Console.ReadLine();