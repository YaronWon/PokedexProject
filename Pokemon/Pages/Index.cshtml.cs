using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Pokemon.Models;

namespace Pokemon.Pages
{
    public class IndexModel : PageModel
    {
        //First login test
        private readonly ILogger<IndexModel> _logger;
        //cache memory
        private readonly IMemoryCache _cache;

        //constructor that get the logger and the cache
        public IndexModel(ILogger<IndexModel> logger , IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache; 
        }

        //pagemodel property
        public List<TypeBasicClass> TypeArr { get; set; }

        public async void OnGet()
        {
            //Checking if the cache memory already have the type list
            var AllTypes = _cache.Get<List<TypeBasicClass>>("MainTypeList");
          
            if(AllTypes == null)
            {
                //the request url
                string TypeUrl = "https://pokeapi.co/api/v2/type";
               
                //http client to get request
                using (var client = new HttpClient())
                {
                    //calling the api , get method 
                    var TypeRequest = client.GetAsync(TypeUrl);
                    //the api response
                    HttpResponseMessage Message = TypeRequest.Result;
                   
                    //checking if the response successed
                    if (Message.IsSuccessStatusCode)
                    {
                        //read the response content
                        var TypeContent = await Message.Content.ReadAsStringAsync();
                        // convert to json object
                        JObject TypCon = JObject.Parse(TypeContent);

                        //get the relvant property from the object and convert it to json array
                        JArray TypeArray = (JArray)TypCon["results"];
                        //creating new instance for the array from the page model property
                        TypeArr = new List<TypeBasicClass>();
                        //loop on the json array
                        foreach (var tempType in TypeArray)
                        {
                            // the type name
                            string tempTypeName = tempType["name"].ToString();
                            // the type url
                            string tempTypeUrl = tempType["url"].ToString();
                             //the type id
                            string tempTypeID = PokemonID(tempTypeUrl);
                            //the image url of the type
                            string ImageTypeUrl = $"~/Images/TypeImages/{tempTypeID}.png";
                            //creating the type class
                            TypeBasicClass temp = new TypeBasicClass(tempTypeName, tempTypeUrl, tempTypeID, ImageTypeUrl);
                            
                            //adding the class to the page model property
                            TypeArr.Add(temp);
                        }
                        //bug fix in the api response
                        TypeArr.RemoveRange(TypeArr.Count - 2, 2);
                         // add the list to the cache memory
                        _cache.Set<List<TypeBasicClass>>("MainTypeList", TypeArr);
                    }
                }
            }
            else
            {
                TypeArr = AllTypes;
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