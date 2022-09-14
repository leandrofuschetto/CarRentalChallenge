namespace CarRental.Domain.Exceptions
{
    public class VehicleInactiveException : Exception
    {
        public string Code { get; private set; } = "VEHICLE_INACTIVE_EXCEPTION";
        public VehicleInactiveException(string message) : base(message)
        { }
    }
}
