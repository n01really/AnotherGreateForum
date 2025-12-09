using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}