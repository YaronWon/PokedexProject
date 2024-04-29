using Newtonsoft.Json;

namespace Pokemon.Models
{
    public class ChainMember
    {
        [JsonProperty("Species")]
        public BasicClass Species { get; set; }
    }
}
