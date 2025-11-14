using congestion.calculator.Domain.Enumerations;
using System.Collections;
using System.Collections.Generic;

namespace congestion.calculator.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;                  
    public int VehicleTypeId { get; set; } = (int)VehicleType.Unknown;

    public VehicleType VehicleType { get; set; } = VehicleType.Unknown;

    public ICollection<VehiclePass> Passes { get; set; } = new List<VehiclePass>();
}
