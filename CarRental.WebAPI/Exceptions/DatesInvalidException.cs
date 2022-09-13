namespace CarRental.WebAPI.Exceptions
{
    public class DatesInvalidException : Exception
    {
        public string Code { get; set; } = "DATEFROM_MAJOR_DATETO";
        public DatesInvalidException(string message) : base(message)
        { }
    }
}
