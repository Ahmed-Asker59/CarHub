namespace API.DTO
{
    public class RentalDTO
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string  Car { get; set; }
        public DateTime RentalDate { get; set; }

        public DateTime EndDate { get; set; }
        public decimal RentalPrice { get; set; }
    }
}
