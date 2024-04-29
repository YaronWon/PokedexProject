using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pokemon.Models
{
    [Keyless]
    public class PokemonList
    {


        [JsonProperty("results")]
        public List<PokemonClass> PokemonsList { get; set; } = new List<PokemonClass>();

        public PokemonList() { }

        public PokemonList(List<PokemonClass> pokemonsList)
        {

            PokemonsList = pokemonsList;
        }
    }
}
