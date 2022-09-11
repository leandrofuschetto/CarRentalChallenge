namespace CarRental.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string Code { get; set; }
        public EntityNotFoundException(string message, string ErrorCode) : base(message)
        {
            this.Code = ErrorCode;
        }
    }
}
