using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokemon.Models
{
    public class PokemonClass
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;
        [NotMapped]
        [JsonProperty("types")]
        public List<PokemonType> Type { get; set; }

        [NotMapped]
        [JsonProperty("chain")]
        public PokemonList Evolution { get; set; }

        [JsonProperty("stats")]
        public List<BaseStat> BaseStat { get; set; }

        public string ImageUrl { get; set; }


        [NotMapped]
        public List<TypeDamageRelations>? ProsAndCons { get; set; } = new List<TypeDamageRelations>();

        public PokemonClass()
        {

        }


        public PokemonClass(int id, string name, string url, List<PokemonType> type, string _Image, List<TypeDamageRelations> prosAndCons)
        {
            Id = id;
            Name = name;
            Url = url;
            Type = type;
            ProsAndCons = prosAndCons;
            ImageUrl = _Image;
        }

    }
}
