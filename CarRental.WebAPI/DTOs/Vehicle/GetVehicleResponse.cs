namespace CarRental.WebAPI.DTOs.Vehicle
{
    public class GetVehicleResponse
    {
        public GetVehicleResponse(Domain.Models.Vehicle vehicle)
        {
            this.VehicleId = vehicle.VehicleId;
            this.Model = vehicle.Model;
            this.PricePerDay = vehicle.PricePerDay;
            this.Active = vehicle.Active;
        }

        public int VehicleId { get; set; }
        public string Model { get; set; }
        public int PricePerDay { get; set; }
        public bool Active { get; set; }
    }
}
