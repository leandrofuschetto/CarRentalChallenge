namespace CarRental.Data.DAOs
{
    public class VehicleDAo
    {
        private readonly CarRentalContext _context;

        public VehicleDAo(CarRentalContext context)
        {
            _context = context;
        }
    }
}
