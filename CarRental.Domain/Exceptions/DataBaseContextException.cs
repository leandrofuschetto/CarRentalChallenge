namespace CarRental.Domain.Exceptions
{
    public class DataBaseContextException : Exception
    {
        public string Code { get; set; } = "DATABASE_GENERAL_EXCEPTION";

        public DataBaseContextException(string message = null): base(message) { }      
    }
}
