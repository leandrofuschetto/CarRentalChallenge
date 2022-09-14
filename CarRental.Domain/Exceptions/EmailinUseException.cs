namespace CarRental.Domain.Exceptions
{
    public class EmailinUseException : Exception
    {
        public string Code { get; private set; } = "EMAIL_UNIQUE_ERROR";
        public EmailinUseException(string message) : base(message)
        { }
    }
}
