using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pokemon.Models
{
    public class PokemonTypeList
    {
        [NotMapped]
        [JsonProperty("pokemon")]
        public List<BasicClass> ClassList { get; set; }

        public PokemonTypeList()
        {

        }

        public PokemonTypeList(List<BasicClass> List)
        {
            ClassList = List;

        }


    }
}
