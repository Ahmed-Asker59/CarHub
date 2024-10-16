using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class StripeSettings
    {
        public int PublishableKey { get; set; }
        public int SecretKey { get; set; }
    }
}
