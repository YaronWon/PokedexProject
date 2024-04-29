using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pokemon.Models
{
    public class BaseStat
    {
        [Key]
        [JsonProperty("base_stat")]
        public int base_stat { get; set; }

        [JsonProperty("effort")]
        public int effort { get; set; }

        [JsonProperty("stat")]
        public BasicClass stat { get; set; }

        public BaseStat()
        {

        }
    }
}
