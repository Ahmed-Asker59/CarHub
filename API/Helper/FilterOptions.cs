using Core.Entities;

namespace API.Helper
{
    public class FilterOptions
    {
        public List<Brand> Makes { get; set; } = null!;
        public List<Model> Models { get; set; } = null!;
        public List<string> Colors { get; set; } = null!;
        public List<string> FuelTypes { get; set; } = null!;
        public List<string> ModelVariants { get; set; } = null!;
        public List<string> CarConditions { get; set; } = null!;
        public List<string> Transmissions { get; set; } = null!;
    }
}
