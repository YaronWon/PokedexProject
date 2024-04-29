using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pokemon.Models
{
    public class BasicClass
    {
        [Key]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

        public BasicClass()
        {

        }
        public BasicClass(string name, string url)
        {
            Name = name;
            Url = url;
        }

    }
}
