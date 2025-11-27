using System.ComponentModel.DataAnnotations;

namespace AnotherGoodAPI.DTOs
{
    public class CategoryCreateDto
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
