using Newtonsoft.Json;

namespace Pokemon.Models
{
    public class TypeList
    {

        public List<TypeBasicClass> Typelist { get; set; }

        public TypeList()
        {

        }
        public TypeList(List<TypeBasicClass> mainTypelist)
        {
            Typelist = mainTypelist;
        }
    }
}
