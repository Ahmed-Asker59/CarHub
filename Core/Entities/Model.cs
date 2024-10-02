using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Model:BaseEntity
    {
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public int MakeId { get; set; }
        public Make Make { get; set; } = null!;
       
    }
}
