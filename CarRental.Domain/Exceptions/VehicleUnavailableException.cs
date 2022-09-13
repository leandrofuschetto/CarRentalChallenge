namespace CarRental.Domain.Exceptions
{
    public class VehicleUnavailableException : Exception
    {
        public string Code { get; set; } = "VEHICLE_UNAVAILABLE_EXCEPTION";
        public VehicleUnavailableException(string message) : base(message)
        { }
    }
}
