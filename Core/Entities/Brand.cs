using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Brand:BaseEntity
    {
        public string Name { get; set; } = null!;

        public string ImagePath { get; set; } = null!;

        public ICollection<Make> Makes { get; set; } = new List<Make>();
    }
}
