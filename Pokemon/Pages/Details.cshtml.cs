using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Pokemon.Models;

namespace Pokemon.Pages
{
    public class DetailsModel : PageModel
    {
        //this page is for showing data on one pokemon 

        //property to the page model that can be used in the razor page
        public PokemonClass CurrentPokemon { get; set; }

        //private property to the cache memory as well
        private readonly IMemoryCache _cache;

        //Consturctor that on the start get the Cache memory object
        public DetailsModel(IMemoryCache cache)
        {

            _cache = cache;
        }

        //to go tothis page you need to send id first
        public async void OnGet(string Id)
        {

            //checking if the cache memroy is already exsits
            if (_cache.Get<PokemonClass>("CurrentPokemon" + Id) == null)
            {
                //using HttpClient to call to the api
                using (HttpClient client = new HttpClient())
                {
                    //The request url from the api and using get method
                    var task = client.GetAsync($"https://pokeapi.co/api/v2/pokemon/{Id}");
                   
                    //the response from the api
                    HttpResponseMessage response = task.Result;

                    //checking if the request  successed
                    if (response.IsSuccessStatusCode)
                    {
                        //get the response content as string and async
                        Task<string> JsonResponse = response.Content.ReadAsStringAsync();

                        //the result from the task
                        var Result = JsonResponse.Result;

                        //convert the content int pokemon class
                        CurrentPokemon = JsonConvert.DeserializeObject<PokemonClass>(Result);
                        //give the pokemon class url for image 
                        CurrentPokemon.ImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{Id}.png";

                        //sending the id to check the pokemon evoultion
                        var EvoulationResult = Pokemon_Evolution(Id);

                        //the evolution function is async so we waiting to the result,
                        //and add it to the pokemon class property
                        CurrentPokemon.Evolution = EvoulationResult.Result;
                       
                        //url: "https://pokeapi.co/api/v2/type/12/"

                        //foreach loop because pokemon can have two types
                        foreach (PokemonType type in CurrentPokemon.Type)
                        {
                            //send the type url to function that extracts the id
                            //and then convert to int because the api set the id as int
                            type.PokeType.Id = Int32.Parse(UrlToId(type.PokeType.URL));

                            // get the damage relations of each type with the type id
                            TypeDamageRelations typeDamageRelations = await PokemonTypeRelation(type.PokeType.Id);

                            //checking that the function returned value
                            if (typeDamageRelations != null)
                            {
                                //setting the damage relations property of the pokemon
                                CurrentPokemon.ProsAndCons.Add(typeDamageRelations);
                            }
                        }

                        //setting the url property of the pokemon
                        CurrentPokemon.Url = $"https://pokeapi.co/api/v2/pokemon/{Id}";
                    }
                }

                //set as the current pokemon that the user see
                
                _cache.Set<PokemonClass>("CurrentPokemon" + Id, CurrentPokemon);

            }
            else
            {
                //if the pokemon called in the past his details is saved
                CurrentPokemon = _cache.Get<PokemonClass>("CurrentPokemon" + Id);
            }
        }


        //function that get the pokemon id and returns is evoluation
        public async Task<PokemonList> Pokemon_Evolution(string PokeId)
        {
            //instance of pokemon list for the result
            PokemonList ResultEvolutionList = new PokemonList();

            //giving the pokemon id to the evoulation chain in cache memory
            string CacheID = "EvoID" + PokeId;

            //checking if the evoulation chain called by other pokemon
            if (_cache.Get<PokemonList>(CacheID) == null)
            {
                //list of strings for the pokemon names
                List<string> EvoulationList = new List<string>();
                
                // the request url with the id 
                string RequestUrl = $"https://pokeapi.co/api/v2/pokemon-species/{PokeId}/";
                
                List<string> EvoIDCache = new List<string>();

                //using http client for the first call
                using (HttpClient httpsClient = new HttpClient())
                {
                    //sending the request to get method
                    var EvoultionRequset = await httpsClient.GetAsync(RequestUrl);

                    //checking if the request successed
                    if (EvoultionRequset.IsSuccessStatusCode)
                    {
                        // read response content
                        var EvoultionResponse = await EvoultionRequset.Content.ReadAsStringAsync();
                        //converting into json object
                        JObject TempResponse = JObject.Parse(EvoultionResponse);
                        //taking only the evolution property from the converted response object
                        JObject EvoultionChain = TempResponse["evolution_chain"].ToObject<JObject>();
                        //taking the url to make a request
                        string EvoultionURL = EvoultionChain["url"].ToString();
                        //checking that the pokemon have evolution
                        if (EvoultionURL != null)
                        {
                            //second call to the api for getting the evolution chain
                            var EvoReq = await httpsClient.GetAsync(EvoultionURL);

                            //checking if the request successed
                            if (EvoReq.IsSuccessStatusCode)
                            {
                                //reading the response content
                                var EvoResponse = await EvoReq.Content.ReadAsStringAsync();
                                //make it json object
                                JObject EvoJson = JObject.Parse(EvoResponse);
                                //going to the chain property
                                JObject EvoChain = EvoJson["chain"].ToObject<JObject>();

                                //checking if not null
                                if (EvoChain != null)
                                {
                                    //getting the id of the first evolution in the chain
                                    string FirstEvoId = UrlToId(EvoChain["species"]["url"].ToString());

                                    
                                    if (FirstEvoId != null)
                                    {
                                        //adding the id to the list of strings
                                        EvoulationList.Add(FirstEvoId);

                                    }

                                    //checking if the pokemon have more evolutions
                                    if (EvoChain["evolves_to"].Count() > 0)
                                    {
                                        //taking the second evolution
                                        var SecondEvolution = EvoChain?["evolves_to"]?[0];

                                        //checking that is not null
                                        if (SecondEvolution != null)
                                        {
                                            //getting the second evolution pokemon id
                                            string SecEvoId = UrlToId(SecondEvolution["species"]["url"].ToString());

                                            if (SecEvoId != null)
                                            {
                                                //adding his id to the list
                                                EvoulationList.Add(SecEvoId);
                                            }


                                        }

                                        //checking if the pokemon have more then 3 pokemons in his evolution chain
                                        if (EvoChain["evolves_to"].Count() >= 2)
                                        {
                                            //taking the amonut of evolutions that exists
                                            int ChainMembers = EvoChain["evolves_to"].Count();

                                            //starting from 1 because pokemon can evolve to one of them
                                            //so the first evolution is exists
                                            for (int ChainPostion = 1; ChainPostion < ChainMembers; ChainPostion++)
                                            {
                                                //getting  the Pokemon id
                                                string temp = UrlToId(EvoChain["evolves_to"][ChainPostion]["species"]["url"].ToString());
                                                
                                                //checking that he exists
                                                if (temp != null)
                                                {
                                                    //addinf the id to the list
                                                    EvoulationList.Add(temp);
                                                }

                                            }


                                        }
                                        //if its a normal structer of pokemon he should have 3 evolutions on his chain
                                        else
                                        {
                                            //going to the last evolution fron the api response
                                            //and from him getting the third pokemon
                                            var ThirdEvoultion = SecondEvolution["evolves_to"].Last;
                                            if (ThirdEvoultion != null)
                                            {
                                                //adding the id of the third pokemon to the list
                                                EvoulationList.Add(UrlToId(ThirdEvoultion["species"]["url"].ToString()));
                                            }
                                        }
                                    }



                                }
                            }

                        }
                    }
                }







                if (EvoulationList != null)
                {

                    //loop on all the id's that we got
                    foreach (string EvoId in EvoulationList)
                    {
                        //creating id to the avolution chain
                        EvoIDCache.Add("EvoId" + EvoId);

                        //http client for calling the api and getting data on the avolutions
                        using (HttpClient HttpSclient = new HttpClient())
                        {
                            //calling api to get the pokemon information
                            var taskEvo = HttpSclient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{EvoId}");
                            //the response from the api
                            HttpResponseMessage Evoresponse = taskEvo.Result;
                            //create instance of pokemon class
                            PokemonClass temp = new PokemonClass();
                            
                            //checking if the response succssed
                            if (Evoresponse.IsSuccessStatusCode)
                            {
                                //read the response content
                                Task<string> JsonResponse = Evoresponse.Content.ReadAsStringAsync();
                               //the result from the reading
                                var Result = JsonResponse.Result;
                                //converting the response to pokemon class
                                temp = JsonConvert.DeserializeObject<PokemonClass>(Result);
                                //givinig him url
                                temp.Url = $"https://pokeapi.co/api/v2/pokemon/{EvoId}";
                                //givinig him url to image
                                temp.ImageUrl = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{EvoId}.png";

                                //adding the pokemn to to the pokemon evoluation chain 
                                ResultEvolutionList.PokemonsList.Add(temp);
                            }

                        }
                    }
                }


                foreach (string CacheEvo in EvoIDCache)
                {
                    //adding the evolution list to the cache 
                    _cache.Set<PokemonList>(CacheEvo, ResultEvolutionList);
                }

            }
            else
            //if the evolution list is already exists
            {

                ResultEvolutionList = _cache.Get<PokemonList>(CacheID);
            }


            return ResultEvolutionList;
        }


        //function that getting string url and return string id
        public string UrlToId(string url)
        {
            // $"https://pokeapi.co/api/v2/type/11/"
            //url for example
            int LastSlash = url.LastIndexOf('/');
            int SecondLastSlash = url.TrimEnd('/').LastIndexOf('/');
            string IdOutput = url.Substring(SecondLastSlash + 1, (LastSlash - SecondLastSlash) - 1);
            return IdOutput;
        }



        //function that getting the type id and returning his strength and weakness
        public async Task<TypeDamageRelations> PokemonTypeRelation(int typeID)
        {
            //instance of damage relations 
            TypeDamageRelations TypeProsAndCons = new TypeDamageRelations();
            //id for the cache memory
            string CacheName = typeID + "-DamageRelations";

            //checking that the cache is exists
            if (_cache.Get<TypeDamageRelations>(CacheName) == null)
            {
                //the request url
                string RequestUrl = $"https://pokeapi.co/api/v2/type/{typeID}";

                //using http client for calling the api
                using (var client = new HttpClient())
                {
                    //calling the api using GET method
                    var typeRequest = await client.GetAsync(RequestUrl);

                    //checking if the call successed
                    if (typeRequest.IsSuccessStatusCode)
                    {
                        //read the rsponse content
                        var typeResponse = await typeRequest.Content.ReadAsStringAsync();
                        //convrting the content to json object
                        JObject tempType = JObject.Parse(typeResponse);
                        //extracting the damage relations property
                        JObject tempProsAndCons = tempType["damage_relations"].ToObject<JObject>();
                        //converting it to Type damage relations type
                        var ProsAndCons = JsonConvert.DeserializeObject<TypeDamageRelations>(tempProsAndCons.ToString());

                        TypeProsAndCons = ProsAndCons;
                    }

                }

                //setting the cache memory for for the type relations
                _cache.Set<TypeDamageRelations>(CacheName , TypeProsAndCons);

            }
            
            else
            {

                TypeProsAndCons = _cache.Get<TypeDamageRelations>(CacheName);
            }

            return TypeProsAndCons;
        }
    }
}





  




    


























































    

       

   




