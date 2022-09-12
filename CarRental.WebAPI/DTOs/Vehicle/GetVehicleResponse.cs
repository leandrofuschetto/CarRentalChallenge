namespace CarRental.WebAPI.DTOs.Vehicle
{
    public class GetVehicleResponse
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int PricePerDay { get; set; }
        public bool Active { get; set; }

        public GetVehicleResponse FromDomain(Domain.Models.Vehicle vehicle)
        {
            this.Id = vehicle.Id;
            this.Model = vehicle.Model;
            this.PricePerDay = vehicle.PricePerDay;
            this.Active = vehicle.Active;

            return this;
        }
    }
}
