using System;
using System.Collections.Generic;
using System.Linq;
using congestion.calculator.Domain.Entities;
using congestion.calculator.Domain.Enumerations;

namespace congestion.calculator.Domain.Services;

public class CongestionTaxCalculator
{
    // HashSet Advantages: fast O(1) lookup, type-safe, easier to maintain and extend
    private static readonly HashSet<VehicleType> TollFreeVehicleTypes = new()
    {
        VehicleType.Emergency,
        VehicleType.Bus,
        VehicleType.Diplomat,
        VehicleType.Motorcycle,
        VehicleType.Military,
        VehicleType.Foreign
    };


    // Accurate list of official public holidays in Gothenburg, Sweden for 2013.
    // Used to identify toll-free dates in congestion tax calculation.
    private static readonly HashSet<DateTime> Holidays2013 = new()
    {
        new DateTime(2013, 1, 1),   // New Year’s Day
        new DateTime(2013, 1, 6),   // Epiphany
        new DateTime(2013, 3, 29),  // Good Friday
        new DateTime(2013, 3, 31),  // Easter Sunday
        new DateTime(2013, 4, 1),   // Easter Monday
        new DateTime(2013, 5, 1),   // Labour Day
        new DateTime(2013, 5, 9),   // Ascension Day
        new DateTime(2013, 5, 19),  // Pentecost
        new DateTime(2013, 6, 6),   // National Day of Sweden
        new DateTime(2013, 6, 21),  // Midsummer Eve
        new DateTime(2013, 6, 22),  // Midsummer Day
        new DateTime(2013, 11, 2),  // All Saints’ Day
        new DateTime(2013, 12, 24), // Christmas Eve
        new DateTime(2013, 12, 25), // Christmas Day
        new DateTime(2013, 12, 26), // Boxing Day
        new DateTime(2013, 12, 31)  // New Year’s Eve
    };


    public int GetTaxForSingleDay(Vehicle vehicle, DateTime[] dates)
    {
        DateTime intervalStart = dates[0];
        int totalFee = 0;
        foreach (DateTime date in dates)
        {
            int nextFee = GetTollFee(date, vehicle);
            int tempFee = GetTollFee(intervalStart, vehicle);

            double minutes = (date - intervalStart).TotalMinutes;

            if (minutes <= 60.0)
            {
                if (totalFee > 0) totalFee -= tempFee;
                if (nextFee >= tempFee) tempFee = nextFee;
                totalFee += tempFee;
            }
            else
            {
                totalFee += nextFee;
                intervalStart = date;
            }
        }

        // Apply daily cap (60 SEK per day as per Gothenburg congestion tax rule)
        if (totalFee > 60) totalFee = 60;

        return totalFee;
    }

    public int GetTax(Vehicle vehicle, DateTime[] dates)
    {
        if (vehicle == null || dates == null || dates.Length == 0) return 0;
        if (IsTollFreeVehicle(vehicle)) return 0;

        // Group by calendar day
        var groupedByDay = dates.OrderBy(d => d).GroupBy(d => d.Date);

        int total = 0;
        foreach (var group in groupedByDay)
        {
            var singleDayDates = group.ToArray();
            total += GetTaxForSingleDay(vehicle, singleDayDates);
        }

        return total;
    }
    
    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        if (vehicle == null) return false;

        return TollFreeVehicleTypes.Contains(vehicle.VehicleType);
    }

    public int GetTollFee(DateTime date, Vehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

        int hour = date.Hour;
        int minute = date.Minute;

        int totalMinutes = hour * 60 + minute;
        if (totalMinutes >= 6 * 60 && totalMinutes <= 6 * 60 + 29) return 8;         // 06:00-06:29
        if (totalMinutes >= 6 * 60 + 30 && totalMinutes <= 6 * 60 + 59) return 13;   // 06:30-06:59
        if (totalMinutes >= 7 * 60 && totalMinutes <= 7 * 60 + 59) return 18;        // 07:00-07:59
        if (totalMinutes >= 8 * 60 && totalMinutes <= 8 * 60 + 29) return 13;        // 08:00-08:29
        if (totalMinutes >= 8 * 60 + 30 && totalMinutes <= 14 * 60 + 59) return 8;   // 08:30-14:59
        if (totalMinutes >= 15 * 60 && totalMinutes <= 15 * 60 + 29) return 13;      // 15:00-15:29
        if (totalMinutes >= 15 * 60 + 30 && totalMinutes <= 16 * 60 + 59) return 18; // 15:30-16:59
        if (totalMinutes >= 17 * 60 && totalMinutes <= 17 * 60 + 59) return 13;      // 17:00-17:59
        if (totalMinutes >= 18 * 60 && totalMinutes <= 18 * 60 + 29) return 8;       // 18:00-18:29

        return 0;
    }

    private bool IsTollFreeDate(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;
        if (date.Month == 7) return true; // July
        if (Holidays2013.Contains(date.Date)) return true;

        // day before a public holiday
        if (Holidays2013.Contains(date.Date.AddDays(1))) return true;

        return false;
    }

}