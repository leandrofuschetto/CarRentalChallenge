namespace CarRental.Domain.Exceptions
{
    public class ClientInactiveException : Exception
    {
        public string Code { get; private set; } = "CLIENT_INACTIVE_EXCEPTION";
        public ClientInactiveException(string message) : base(message)
        { }
    }
}
