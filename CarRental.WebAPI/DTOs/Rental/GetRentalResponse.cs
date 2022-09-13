﻿using CarRental.WebAPI.DTOs.Client;
using CarRental.WebAPI.DTOs.Vehicle;

namespace CarRental.WebAPI.DTOs.Rental
{
    public class GetRentalResponse
    {
        public int Id { get; set; }
        public GetVehicleResponse Vehicle { get; set; }
        public GetClientResponse Client { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }

        public static GetRentalResponse FromDomain(Domain.Models.Rental rental)
        {
            GetRentalResponse response = new();

            response.Id = rental.Id;
            response.Client = new GetClientResponse()
            {
                Id = rental.Client.Id,
                Email = rental.Client.Email,
                Fullname = rental.Client.Fullname
            };
            response.Vehicle = new GetVehicleResponse()
            {
                Id = rental.Vehicle.Id,
                Model = rental.Vehicle.Model,
                PricePerDay = rental.Vehicle.PricePerDay
            };
            response.DateFrom = rental.DateFrom;
            response.DateTo = rental.DateTo;
            response.Price = rental.Price;
            response.Active = rental.Active;

            return response;
        }
    }
}
