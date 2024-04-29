using System.ComponentModel.DataAnnotations;

namespace Pokemon.Models
{
    public class TypeBasicClass : BasicClass
    {
       
        public string TypePokemonID { get; set; }
        public string ImgUrl { get; set; }

        public TypeBasicClass() : base()
        {

        }
     
        public TypeBasicClass(string name, string url, string Pokeid, string imgUrl) : base(name, url)
        {

            TypePokemonID = Pokeid;
            ImgUrl = imgUrl;
        }
    }
}
