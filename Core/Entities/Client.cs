using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string NationalId { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; } = null!;
        [Required]
        [StringLength(20)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(30)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(11)]
        public string Phone { get; set; } = null!;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    }
}