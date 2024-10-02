using API.DTO;
using AutoMapper;
using Core.Entities;

namespace API.Helper
{
    public class CarUrlResoler : IValueResolver<Car, CarDTO, string>
    {

        private readonly IConfiguration _Config;
        public CarUrlResoler(IConfiguration config)
        {
            _Config = config;

        }
        //public Type ResolvedType => throw new NotImplementedException();

        //public Expression GetExpression(IGlobalConfiguration configuration, MemberMap memberMap, Expression source, Expression destination, Expression destinationMember)
        //{
        //    throw new NotImplementedException();
        //}

        //public MemberInfo GetSourceMember(MemberMap memberMap)
        //{
        //    throw new NotImplementedException();
        //}

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
