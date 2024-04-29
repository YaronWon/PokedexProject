using System.ComponentModel.DataAnnotations;

namespace Pokemon.Models
{
    public class Type
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string URL { get; set; } = string.Empty;

        public Type()
        {

        }

        public Type(int id, string name, string url)
        {
            Id = id;
            Name = name;
            URL = url;
        }
    }
}
