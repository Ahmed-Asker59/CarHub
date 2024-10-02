using Core.Entities.Consts;
using Core.Entities;

namespace API.DTO
{
    public class CarDTO
    {

        public int BrandId { get; set; }

        public Brand Brand { get; set; } = null!;
        public int MakeId { get; set; }

        public Make Make { get; set; } = null!;
        public int ModelId { get; set; }
        public Model Model { get; set; } = null!;
        public ModelVariant ModelVariant { get; set; }
        public string Color { get; set; } = null!;
        public TransmissionType Transmission { get; set; }
        public FuelType Fuel { get; set; }
        public CarCondition CarCondition { get; set; }
        public decimal Price { get; set; }
        public int ManufactureYear { get; set; }
        public bool IsAvailableForReserve { get; set; }
        public bool IsAvailableForRental { get; set; }
        public string? ImagePath { get; set; }
    }
}
