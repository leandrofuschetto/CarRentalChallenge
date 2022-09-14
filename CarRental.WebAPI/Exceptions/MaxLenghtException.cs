namespace CarRental.WebAPI.Exceptions
{
    public class MaxLenghtException : Exception
    {
        public string Code { get; set; }
        public MaxLenghtException(string message, string ErrorCode) 
            : base(message)
        {
            this.Code = ErrorCode;
        }
    }
}
