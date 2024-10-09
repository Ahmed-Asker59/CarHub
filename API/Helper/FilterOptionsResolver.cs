using API.DTO;
using Core.Entities;

namespace API.Helper
{
    public class FilterOptionsResolver
    {
        private readonly IEnumerable<Car> cars;

        public FilterOptionsResolver(IEnumerable<Car> cars)
        {
            this.cars = cars;

        }

        public FilterOptions GenerateOptions()
        {
            var filterOptions = new FilterOptions
            {
                Makes = cars.Select(c => c.Brand).Distinct().ToList(),
                Models = cars.Select(c => c.Model).Distinct().ToList(),
                Colors = cars.Select(c => c.Color).Distinct().ToList(),
                FuelTypes = cars.Select(c => c.Fuel.ToString()).Distinct().ToList(),
                ModelVariants = cars.Select(c => c.ModelVariant.ToString()).Distinct().ToList(),
                Transmissions = cars.Select(c => c.Transmission.ToString()).Distinct().ToList(),
                CarConditions = cars.Select(c => c.CarCondition.ToString()).Distinct().ToList()
            };

            return filterOptions;
        }
    }
}
