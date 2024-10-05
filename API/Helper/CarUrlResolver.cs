using API.DTO;
using AutoMapper;
using Core.Entities;

namespace API.Helper
{
    public class CarUrlResolver : IValueResolver<Car, CarDTO, string>
    {

        private readonly IConfiguration _Config;
        public CarUrlResolver(IConfiguration config)
        {
            _Config = config;

        }
       
        public string Resolve(Car source, CarDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImagePath))
            {
                return _Config["ApiUrl"] + source.ImagePath;
            }
            return null;
        }
    }
}
