using Newtonsoft.Json;

namespace Pokemon.Models
{
    public class ChainLink
    {
        [JsonProperty("evolves_to")]
        public ChainMember ChainMember { get; set; }
    }
}
