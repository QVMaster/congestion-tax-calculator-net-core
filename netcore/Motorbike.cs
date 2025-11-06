using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace congestion.calculator
{
    public class Motorbike : IVehicle
    {
        // Previous: used method returning string. 
        // Issues: prone to typos, less type-safe, harder to maintain and test.
        //public string GetVehicleType()
        //{
        //    return "Motorbike";
        //}

        // Fix: use property returning enum VehicleType. 
        // Benefits: type-safe, more readable, easier to maintain and test.
        public VehicleType Type => VehicleType.Motorbike;
    }
}