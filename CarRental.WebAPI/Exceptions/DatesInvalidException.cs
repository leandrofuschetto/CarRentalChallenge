namespace CarRental.WebAPI.Exceptions
{
    public class DatesInvalidException : Exception
    {
        public string Code { get; private set; } = "DATES_INVALID_EXCEPTION";
        public DatesInvalidException(string message) : base(message)
        { }
    }
}
