
namespace congestion.calculator;

public enum VehicleType { Car, Motorbike, Motorcycle, Tractor, Truck, Emergency, Diplomat, Foreign, Military, Bus, }

// Renamed 'Vehicle' to 'IVehicle' for clarity and consistency with C# interface naming conventions.
public interface IVehicle
{
    // Old: using string
    //String GetVehicleType();

    // Fix: Change return type
    VehicleType Type { get; }
}