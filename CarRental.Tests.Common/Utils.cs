using AutoMapper;
using CarRental.Mapper;

namespace CarRental.Tests.Helpers
{
    public class Utils
    {
        public IMapper GetMapper()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });

            var mapper = mockMapper.CreateMapper();

            return mapper;
        }
    }
}
