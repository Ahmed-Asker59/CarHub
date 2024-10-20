using Core.Entities;

namespace API.DTO
{
    public class RentalToAlertDTO
    {
        public Car Car { get; set; }
        public DateTime EndDate { get; set; }
        
    }
}
