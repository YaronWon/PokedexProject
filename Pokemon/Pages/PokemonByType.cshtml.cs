using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pokemon.Models;
using System.Text.Json.Serialization;

namespace Pokemon.Pages
{
    public class PokemonByTypeModel : PageModel
    {
        //list for all the pokemons that from the same type
        public List<TypeBasicClass> TypePokemonList { get; set; } 
        //the damage relations of the type
        public TypeDamageRelations TypeDamageRelations { get; set; }
        //the name of the type
        public string TypeName { get; set; }
        //the id of the type
        public string TypeID { get; set; }
        //property for cache memory
        private readonly IMemoryCache _cache;

        //The page model constructor
        public PokemonByTypeModel(IMemoryCache cache)
        {
            _cache = cache;
        }


        public void OnGet(string id)
        {
           
            var CacheTypeRelation = _cache.Get<TypeDamageRelations>(id + "-DamageRelations");
            var CacheTypePokemons = _cache.Get<List<TypeBasicClass>>(id + "-PokemonsType");
            var CacheTName = _cache.Get<string>(id + "-TypeName");
            //query for the cache id
            var CacheTypeID = Request.Query["id"].ToString();

            //checking if the cache is already exists
            if (CacheTypeRelation == null || CacheTypePokemons == null || CacheTypeID == null) 
            {
                //http client to call the api
                using (HttpClient client = new HttpClient())
                {
                    //the request url from the api , get method
                    var Typetask = client.GetAsync($"https://pokeapi.co/api/v2/type/{id}/");
                    //the response from the api
                    HttpResponseMessage response = Typetask.Result;
                    //checking if the api responsed
                    if (response.IsSuccessStatusCode)
                    {
                        //read the response content
                        Task<string> Typeresponse = response.Content.ReadAsStringAsync();
                        //getting the result
                        var TyperesponseJson = Typeresponse.Result;
                        //creating json object
                        JObject TypeData = JObject.Parse(TyperesponseJson);
                        
                        if (TyperesponseJson != null)
                        {
                            //the page model property equal to the json object type name
                            TypeName = TypeData["name"].ToString();
                            //the page model property equal to the json object type id
                            TypeID = TypeData["id"].ToString();
                            //making the damage relations json object 
                            JObject TypeDamageRel = (JObject)TypeData["damage_relations"];
                            //json array of all the pokemon from the same type
                            JArray PokemonTypeArray = (JArray)TypeData["pokemon"];
                            //converting the page property to the class that equal to the page model property
                            TypeDamageRelations = JsonConvert.DeserializeObject<TypeDamageRelations>(TypeDamageRel.ToString());
                            
                            var temps = JsonConvert.SerializeObject(PokemonTypeArray);
                            List<JToken> PokemonTypeList = PokemonTypeArray.ToList();
                            //convert the pokemon type array to list of json tokens
                            
                            //creating new instance of the page model property
                            TypePokemonList = new List<TypeBasicClass>();

                            //loop on the array of json tokens
                            foreach (var PokemnTypeCell in PokemonTypeArray)
                            {
                                var tempUrl = PokemnTypeCell["pokemon"]["url"].ToString();
                                string tempName = PokemnTypeCell["pokemon"]["name"].ToString();
                                var tempId = PokemonID(tempUrl);
                                var tempImgUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{tempId}.png";
                                //creating a class of each pokemon in the json array
                                TypeBasicClass tempos = new TypeBasicClass(tempName, tempUrl, tempId, tempImgUrl);

                                //adding them to the page model property
                                TypePokemonList.Add(tempos);


                            }

                            string CacheTypeName = id + "-TypeName";
                            string CacheTypePokemon = id + "-PokemonsType";
                            string CacheTypeRelations =id + "-DamageRelations";
                            _cache.Set<string>("TypeId", TypeID);
                            _cache.Set<string>(CacheTypeName, TypeName);
                            _cache.Set<List<TypeBasicClass>>(CacheTypePokemon, TypePokemonList);
                            _cache.Set<TypeDamageRelations>(CacheTypeRelations, TypeDamageRelations);
                            //adding all the values to the cache memory

                        }

                    }
                }

            }
            else
            {
                TypePokemonList = CacheTypePokemons;
                TypeDamageRelations = CacheTypeRelation;
                TypeID = CacheTypeID;
                TypeName = CacheTName;
            }
        }


        public string PokemonID(string URL)
        {


            int LastSlash = URL.LastIndexOf('/');
            int nextLastSlash = URL.TrimEnd('/').LastIndexOf('/');
            string ID = URL.Substring(nextLastSlash + 1, (LastSlash - nextLastSlash) - 1);


            return ID;
        }
    }
}


