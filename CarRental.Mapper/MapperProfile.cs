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

            CreateMap<Rental, RentalEntity>()
                .ForMember(d => d.ClientId, opt => opt.MapFrom(src => src.Client.Id))
                .ForMember(d => d.VehicleId, opt => opt.MapFrom(src => src.Vehicle.Id));

            CreateMap<RentalEntity, Rental>();
        }
    }
}