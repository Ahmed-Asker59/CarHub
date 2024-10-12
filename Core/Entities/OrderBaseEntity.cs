using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class OrderBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Car")]
        public int CarId { get; set; }
        public Car Car { get; set; }=null!;

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime EndDate { get; set; } 


    }
}