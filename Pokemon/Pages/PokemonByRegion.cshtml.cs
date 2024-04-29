using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pokemon.Models;

namespace Pokemon.Pages
{
    public class PokemonByRegionModel : PageModel
    {

        /*
           /*
         https://pokeapi.co/api/v2/pokedex/id/
         Kanto - 2
         original-johto - 3
         hoenn - 4
        original-sinnoh - 5
        extended-sinnoh - 6 
        updated-johto - 7

         */

        public List<TypeBasicClass> RegionPokemonList { get; set; }
        public string RegionName { get; set; }
        private readonly IMemoryCache _cache;

        public PokemonByRegionModel(IMemoryCache cache)
        {
            _cache = cache;
        }


        public async void OnGet(string RegionID)
        {
          

            var ListOfRegionPokemons = _cache.Get<List<TypeBasicClass>>(RegionID + "-RegionPokemons");
            var NameOfRegion = _cache.Get<string>("RegionName");

            //checking if the cache memory exsits
            if(ListOfRegionPokemons == null || NameOfRegion == null)
            {
                //the request url
                string url = $"https://pokeapi.co/api/v2/pokedex/{RegionID}/";
                //creating http client
                using (HttpClient client = new HttpClient())
                {
                    //request url to get get method
                    var RegionRequest = client.GetAsync(url);
                    //the response from the api
                    HttpResponseMessage RegionResponse = RegionRequest.Result;
                    //checking if the request successed
                    if (RegionResponse.IsSuccessStatusCode)
                    {
                        //reading the response content
                        var regionContent = await RegionResponse.Content.ReadAsStringAsync();
                        //creating json object and converting the response content
                        JObject ContentReg = JObject.Parse(regionContent);
                        //extarcting the pokemon list that have the type
                        var RegionPokemons = ContentReg["pokemon_entries"];
                        //extracting the name of the reigon and he equal to the page mpdel property
                        RegionName = ContentReg["name"].ToString();
                        //creating new instance of the list in the page model property
                        RegionPokemonList = new List<TypeBasicClass>();
                        //loop on the pokemons from the same region
                        for (int i = 0; i < RegionPokemons.Count(); i++)
                        {

                            string TempUrl = RegionPokemons[i]["pokemon_species"]["url"].ToString();
                            string TempName = RegionPokemons[i]["pokemon_species"]["name"].ToString();
                            string TempID = UrlToId(TempUrl);
                            string TempImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{TempID}.png";
                          
                            TypeBasicClass tempPokemon = new TypeBasicClass(TempName, TempUrl, TempID, TempImageUrl);
                            //creating a temp class 

                            //adding the temp class to the page model property list
                            RegionPokemonList.Add(tempPokemon);
                        }
                    }


                }

                _cache.Set<List<TypeBasicClass>>(RegionID + "-RegionPokemons", RegionPokemonList);
                _cache.Set<string>("RegionName", RegionName);
                //adding the valued to the cache memory
            }
            else
            {
                RegionPokemonList = ListOfRegionPokemons;
                RegionName = NameOfRegion;
            }
        
        
        }

        public string UrlToId(string Url)
        {
            int LastSlash = Url.LastIndexOf('/');
            int nextLastSlash = Url.TrimEnd('/').LastIndexOf('/');
            string ID = Url.Substring(nextLastSlash + 1, (LastSlash - nextLastSlash) - 1);


            return ID;
        }
    }
}
