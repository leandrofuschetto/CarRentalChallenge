namespace CarRental.Domain.Exceptions
{
    public class DataBaseContextException : Exception
    {
        public string Code { get; set; } = "EMAIL_UNIQUE_ERROR";
    }
}
