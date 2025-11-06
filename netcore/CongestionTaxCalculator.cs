using congestion.calculator;
using System;
using System.Collections.Generic;
using System.Linq;



public class CongestionTaxCalculator
{
    /**
         * Calculate the total toll fee for one day
         *
         * @param vehicle - the vehicle
         * @param dates   - date and time of all passes on one day
         * @return - the total congestion tax for that day
         */


    // Previous enum: used for toll-free vehicle types
    // Replaced by HashSet for better maintainability, type safety, and O(1) lookup
    //private enum TollFreeVehicles
    //{
    //    Motorcycle = 0,
    //    Tractor = 1,
    //    Emergency = 2,
    //    Diplomat = 3,
    //    Foreign = 4,
    //    Military = 5
    //}

    // Refactor: HashSet of toll-free vehicle types
    // Advantages: fast O(1) lookup, type-safe, easier to maintain and extend
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


    // Refactored version of the original GetTax method, renamed to GetTaxForSingleDay
    // to clearly indicate that it calculates the congestion tax for a single day.
    // The original GetTax now becomes a higher-level method that handles multiple days
    // and calls this method for each day.    
    //public int GetTax(IVehicle vehicle, DateTime[] dates)
    public int GetTaxForSingleDay(IVehicle vehicle, DateTime[] dates)
    {
        DateTime intervalStart = dates[0];
        int totalFee = 0;
        foreach (DateTime date in dates)
        {
            int nextFee = GetTollFee(date, vehicle);
            int tempFee = GetTollFee(intervalStart, vehicle);

            // Previous (buggy): only compares millisecond components (0–999), not total difference.
            //long diffInMillies = date.Millisecond - intervalStart.Millisecond;
            //long minutes = diffInMillies / 1000 / 60;

            // Fix: use exact time difference for 60-minute rule            
            double minutes = (date - intervalStart).TotalMinutes;


            // Because in the real congestion tax rule,
            // the phrase "within 60 minutes" means any time strictly less than or equal to 60 minutes.
            // Using double preserves the exact precision in comparing times,
            // even if the difference is, say, 60.01 minutes.

            // Previous (buggy): only 
            //if (minutes <= 60)

            // Fix: (date - intervalStart).TotalMinutes preserves fractional minutes.
            // The regulation is interpreted as "within 60 minutes" (<= 60.00 min).
            // Using double ensures that 60.01 min is treated as > 60 min.
            if (minutes <= 60.0)
            {
                if (totalFee > 0) totalFee -= tempFee;
                if (nextFee >= tempFee) tempFee = nextFee;
                totalFee += tempFee;
            }
            else
            {
                totalFee += nextFee;
            }
        }
        if (totalFee > 60) totalFee = 60;

        return totalFee;
    }


    public int GetTax(IVehicle vehicle, DateTime[] dates)
    {
        if (vehicle == null || dates == null || dates.Length == 0) return 0;
        if (IsTollFreeVehicle(vehicle)) return 0;

        // Group by calendar day
        var grouped = dates.OrderBy(d => d).GroupBy(d => d.Date);

        int total = 0;
        foreach (var group in grouped)
        {
            var dayDates = group.ToArray();
            total += GetTaxForSingleDay(vehicle, dayDates);

            // optional: if total grows huge and you want micro-opt, apply daily cap inside GetTaxForSingleDay
        }

        return total;
    }
    
    private bool IsTollFreeVehicle(IVehicle vehicle)
    {
        if (vehicle == null) return false;

        // Previous (ambiguous): logic was hard to read, maintain and test 
        // and did not match the Tax Exempt vehicles list in the assignment
        //String vehicleType = vehicle.GetVehicleType();
        //return vehicleType.Equals(TollFreeVehicles.Motorcycle.ToString()) ||
        //       vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
        //       vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
        //       vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
        //       vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
        //       vehicleType.Equals(TollFreeVehicles.Military.ToString());


        // Fix 1: Replace string comparison with enum for type-safety, maintainability, and testability
        //switch (vehicle.GetVehicleType())
        //{
        //    case VehicleType.Emergency:
        //    case VehicleType.Bus:
        //    case VehicleType.Diplomat:
        //    case VehicleType.Motorcycle:
        //    case VehicleType.Military:
        //    case VehicleType.Foreign:
        //        return true;
        //    default:
        //        return false;
        //}

        // Fix 2: Use HashSet<VehicleType> for toll-free vehicle types for faster lookup, 
        // simpler maintenance, and easier extension of the list
        return TollFreeVehicleTypes.Contains(vehicle.Type);
    }

    public int GetTollFee(DateTime date, IVehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

        int hour = date.Hour;
        int minute = date.Minute;

        // Previous (ambiguous): logic was hard to read and maintain
        //if (hour == 6 && minute >= 0 && minute <= 29) return 8;
        //else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
        //else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
        //else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
        //else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return 8;
        //else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
        //else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
        //else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
        //else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
        //else return 0;

        // Fix: calculate minutes since midnight instead of checking hour+minute separately
        // This removes ambiguity and ensures correct range matching for all time intervals.
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

    private Boolean IsTollFreeDate(DateTime date)
    {
        // Previous implementation: Used simple string or date comparisons without centralized holiday management.
        // This version was less maintainable and required manual updates for each condition.
        //int year = date.Year;
        //int month = date.Month;
        //int day = date.Day;
        //if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;
        //if (year == 2013)
        //{
        //    if (month == 1 && day == 1 ||
        //        month == 3 && (day == 28 || day == 29) ||
        //        month == 4 && (day == 1 || day == 30) ||
        //        month == 5 && (day == 1 || day == 8 || day == 9) ||
        //        month == 6 && (day == 5 || day == 6 || day == 21) ||
        //        month == 7 ||
        //        month == 11 && day == 1 ||
        //        month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
        //    {
        //        return true;
        //    }
        //}
        //return false;


        // Updated implementation: Uses a pre-defined HashSet<DateTime> containing official 2013 Gothenburg public holidays
        // for precise, maintainable, and fast toll-free date checks. Improves clarity and reduces hardcoded conditions.
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;
        if (date.Month == 7) return true; // July
        if (Holidays2013.Contains(date.Date)) return true;

        // day before a public holiday
        if (Holidays2013.Contains(date.Date.AddDays(1))) return true;

        return false;
    }



}