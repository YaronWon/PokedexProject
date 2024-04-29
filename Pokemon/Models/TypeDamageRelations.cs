using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Pokemon.Models
{
    [Keyless]
    public class TypeDamageRelations
    {
        [JsonProperty("no_damage_to")]
        public List<TypeBasicClass>? NoDamageTo { get; set; }
        [JsonProperty("no_damage_from")]
        public List<TypeBasicClass>? NoDamageFr { get; set; }
        [JsonProperty("half_damage_to")]
        public List<TypeBasicClass>? HalfDamageTo { get; set; }
        [JsonProperty("half_damage_from")]
        public List<TypeBasicClass>? HalfDamageFr { get; set; }
        [JsonProperty("double_damage_to")]
        public List<TypeBasicClass>? DoubleoDamageTo { get; set; }
        [JsonProperty("double_damage_from")]
        public List<TypeBasicClass>? DoubleDamageFr { get; set; }


    }
}
