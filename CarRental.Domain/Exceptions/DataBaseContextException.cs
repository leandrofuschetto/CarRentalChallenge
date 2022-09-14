namespace CarRental.Domain.Exceptions
{
    public class DataBaseContextException : Exception
    {
        public string Code { get; private set; } = "DATABASE_GENERAL_EXCEPTION";

        public DataBaseContextException(string message = null): base(message) { }

        public DataBaseContextException(
            string message, 
            Exception innerException = null)
            : base(message, innerException) { }
    }
}
