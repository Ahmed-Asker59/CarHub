using API.DTO;
using AutoMapper;
using Core.Entities;

namespace API.Helper
{
    public class FilterOptionsResolver
    {
        private readonly IEnumerable<Car> _cars;
        private readonly IMapper _mapper;

        public FilterOptionsResolver(IEnumerable<Car> cars, IMapper mapper)
        {
            _cars = cars;
            _mapper = mapper;
        }

        public FilterOptions GenerateOptions()
        {
            var makes = _cars.Select(c => c.Make).Distinct().ToList();
            var models = _cars.Select(c => c.Model).Distinct().ToList();
            var filterOptions = new FilterOptions
            {
                Makes = _mapper.Map<List<MakeDTO>>(makes),
                Models = _mapper.Map<List<ModelDTO>>(models),
                Colors = _cars.Select(c => c.Color).Distinct().ToList(),
                FuelTypes = _cars.Select(c => c.Fuel.ToString()).Distinct().ToList(),
                ModelVariants = _cars.Select(c => c.ModelVariant.ToString()).Distinct().ToList(),
                Transmissions = _cars.Select(c => c.Transmission.ToString()).Distinct().ToList(),
                CarConditions = _cars.Select(c => c.CarCondition.ToString()).Distinct().ToList()
            };

            return filterOptions;
        }
    }
}
