namespace API.DTO
{
    public class ReservationDTO
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string Car { get; set; }
        public DateTime ReservationDate { get; set; }
        public decimal ReservationFee { get; set; } 
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
