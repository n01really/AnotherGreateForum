using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Another_Great_Forum.Pages.Shared
{
    public class CreateForumModel : PageModel
    {
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        public record CategoryDto(int Id, string Name, string Description);



        public void OnGet()
        {
            // Categories will be loaded via JS fetch from backend
        }
    }
}
