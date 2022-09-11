namespace CarRental.Domain.Exceptions
{
    public class EmailinUseException : Exception
    {
        public string Code { get; set; } = "EMAIL_UNIQUE_ERROR";
        public EmailinUseException(string message) : base(message)
        { }
    }
}
