namespace CarRental.Domain.Exceptions
{
    public class UsernameInUseException : Exception
    {
        public string Code { get; set; } = "USERNAME_IN_USE";
        public UsernameInUseException(string message) : base(message)
        { }
    }
}
