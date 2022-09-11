namespace CarRental.Data.DAOs
{
    internal class ClientDao
    {
        private readonly CarRentalContext _context;

        public ClientDao(CarRentalContext context)
        {
            _context = context;
        }
    }
}
