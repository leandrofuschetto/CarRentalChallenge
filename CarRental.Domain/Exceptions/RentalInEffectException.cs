namespace CarRental.Domain.Exceptions
{
    public class RentalInEffectException : Exception
    {
        public string Code { get; set; } = "RENTAL_INEFFECT_EXCEPTION";
        public RentalInEffectException(string message) : base(message)
        { }
    }
}

