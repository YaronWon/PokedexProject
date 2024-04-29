using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Pokemon.Models
{
    [Keyless]
    public class PokemonType
    {
        [JsonProperty("slot")]
        public int Slot { get; set; }
        [JsonProperty("type")]
        public Type? PokeType { get; set; }

    }
}
