namespace CarRental.Data.DAOs
{
    public class RentalDao
    {
        private readonly CarRentalContext _context;

        public RentalDao(CarRentalContext context)
        {
            _context = context;
        }
    }
}
