namespace API.DTO
{
    public class ReserveResponseDTO
    {
        public bool IsAllowed { get; set; }
        public string Message { get; set; } = null!;
    }
}
