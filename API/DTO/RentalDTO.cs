namespace API.DTO
{
    public class RentalDTO
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string  Car { get; set; }
        public DateTime RentalDate { get; set; }

        public DateTime EndDate { get; set; }
        public decimal RentalPrice { get; set; }
      
        public decimal LateFee { get; set; }
        public decimal TotalRentalPrice { get; set; }
        public int DelayInDays { get; set; }
        public bool IsActive { get; set; }

        public DateTime ActualReturnDate { get; set; }
    }
}
