namespace CarRental.WebAPI.Exceptions
{
    public class FieldMandatoryException : Exception
    {
        public string Code { get; set; }
        public FieldMandatoryException(string message, string ErrorCode) 
            : base(message)
        {
            this.Code = ErrorCode;
        }
    }
}
