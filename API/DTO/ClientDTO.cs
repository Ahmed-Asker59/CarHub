namespace API.DTO
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string NationalId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
