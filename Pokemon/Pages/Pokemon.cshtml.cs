using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Pokemon.Models;

namespace Pokemon.Pages
{
    public class PokemonModel : PageModel
    {
    //This is the Main Pokemon List Page

       //property to the page model that can be used in the razor page
        public PokemonList MainPokemonList { get; set; }
        
        //Private Property for the Cache memory
        private readonly IMemoryCache _cache;

        //Consturctor that on the start get the Cache memory object
        public PokemonModel(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnGet()
        {
            //Each result of api call in this app is saved in the cache memory to save calls to the api

            //checking if the cache memroy is already exsits
            if(_cache.Get<PokemonList>("MainPokemonList") == null)
            {
                //using HttpClient to call to the api
                using (HttpClient client = new HttpClient())
                {
                    //The request url from the api
                    string MainCallURL = $"https://pokeapi.co/api/v2/pokemon?limit=10000&offset=0";
                    
                    //calling to the api
                    var task = client.GetAsync(MainCallURL);

                    //the response from the api
                    HttpResponseMessage response = task.Result;

                    //checking if the response sucseed
                    if (response.IsSuccessStatusCode)
                    {
                        //the response content and make it Json string
                        Task<string> JsonResponse = response.Content.ReadAsStringAsync();
                       
                        //the result from the call
                        var Result = JsonResponse.Result;

                        //convert the result to Pokemonlist type , And make it property
                        MainPokemonList = JsonConvert.DeserializeObject<PokemonList>(Result);

                    }

                    //foreach loop in the main pokemon list to give them id according to their url
                    foreach (PokemonClass Pokemon in MainPokemonList.PokemonsList)
                    {
                        Pokemon.Id = Int32.Parse(PokemonID(Pokemon.Url));
                        Pokemon.ImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{Pokemon.Id}.png";
                    }
                }

                //savig the pokemon list in the cache memory
                _cache.Set<PokemonList>("MainPokemonList" , MainPokemonList);

            }
            else
            {
                //else if the cache memory is alreade exsits
                MainPokemonList = _cache.Get<PokemonList>("MainPokemonList");
            }




        }

        //function that get url sring and return the id as string 
        public string PokemonID(string URL)
        {
            //url example:https://pokeapi.co/api/v2/pokemon/234/
            // all the the url that i get from the call endes with slash
            //the id that i need is btween the last slash and the slash before him

            //getting the postion of the last slash in the string
            int LastSlash = URL.LastIndexOf('/');

            ////getting the postion of the the last slash before  last slash in the string
            int nextLastSlash = URL.TrimEnd('/').LastIndexOf('/');

            //extracting the id between them 
            string ID =  URL.Substring(nextLastSlash+1 , (LastSlash-nextLastSlash) - 1);

            //return pokemon id
            return ID;
        }
    }
}
