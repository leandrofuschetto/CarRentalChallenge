namespace CarRental.Domain.Exceptions
{
    public class ModelVehicleInUseException : Exception
    {
        public string Code { get; private set; } = "MODEL_UNIQUE_ERROR";
        public ModelVehicleInUseException(string message) : base(message)
        { }
    }
}
