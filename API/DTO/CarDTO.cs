using Core.Entities.Consts;
using Core.Entities;

namespace API.DTO
{
    public class CarDTO
    {
        public int Id { get; set; } 
        public string Name { get; set; } = null!;
       
        public ModelVariant ModelVariant { get; set; }
        public string Color { get; set; } = null!;
        public TransmissionType Transmission { get; set; }
        public FuelType Fuel { get; set; }
        public CarCondition CarCondition { get; set; }
        public decimal Price { get; set; }
        public int ManufactureYear { get; set; }
        public int Mileage { get; set; }

        public bool IsAvailableForReserve { get; set; }

        public bool IsReserved { get; set; }
        public bool IsRented { get; set; }
        public decimal? ReservationFee { get; set; }
        public decimal? RentalFeePerDay { get; set; }
        public bool IsAvailableForRental { get; set; }

        public string? ImagePath { get; set; }
        
    }
}
