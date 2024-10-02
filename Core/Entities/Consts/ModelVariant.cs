using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entities.Consts
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModelVariant
    {
        Saloon,
        Hatchback,
        Estate,
        Coupe,
        SUV,
        Convertible,
        Crossover,
        MPV, // Multi-Purpose Vehicle
        SpecialEdition 
    }
}
