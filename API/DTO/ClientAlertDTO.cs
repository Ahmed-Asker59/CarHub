namespace API.DTO
{
    public class ClientAlertDTO
    {
  
            public int Id { get; set; }
            public string Name { get; set; }
            public List<RentalToAlertDTO> Rentals { get; set; }
        
    }
}
