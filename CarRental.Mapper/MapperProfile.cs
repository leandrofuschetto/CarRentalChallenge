using AutoMapper;
using CarRental.Data.Entities;
using CarRental.Domain.Models;

namespace CarRental.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Client, ClientEntity>();
            CreateMap<ClientEntity, Client>();

            CreateMap<Vehicle, VehicleEntity>();
            CreateMap<VehicleEntity, Vehicle>();

            CreateMap<Rental, RentalEntity>();
            CreateMap<RentalEntity, Rental>();
        }
    }
}