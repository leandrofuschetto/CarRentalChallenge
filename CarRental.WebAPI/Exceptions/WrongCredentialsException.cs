namespace CarRental.WebAPI.Exceptions
{
    public class WrongCredentialsException : Exception
    {
        public string Code { get; private set; } = "WRONG_CREDENTIALS";
        public WrongCredentialsException(string message)
            : base(message)
        { }
    }
}
