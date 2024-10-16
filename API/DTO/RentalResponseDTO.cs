namespace API.DTO
{
    public class RentalResponseDTO
    {
        public bool IsAllowed { get; set; }
        public string Message { get; set; } = null!;
    }
}
