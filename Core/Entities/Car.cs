using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Consts;

namespace Core.Entities
{
    public class Car:BaseEntity
    {

        // Relationships
        [Required]
        public int BrandId { get; set; } // Foreign key
        public Brand Brand { get; set; } = null!;

        [Required]
        public int MakeId { get; set; } //Foregin Key
        public Make Make { get; set; } = null!;

        [Required]
        public int ModelId { get; set; } // Foreign key
        public Model Model { get; set; } = null!;



        public ModelVariant ModelVariant { get; set; } 


        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = null!;


        public TransmissionType Transmission { get; set; }
        public FuelType Fuel { get; set; }
        public CarCondition CarCondition { get; set; }


        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }


        [Required]
        [Range(1886, int.MaxValue)]
        public int ManufactureYear { get; set; }


        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        
        public string? ImagePath { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
        
        public bool IsAvailableForReserve { get; set; }
        public bool IsAvailableForRental { get; set; }


    }
}
