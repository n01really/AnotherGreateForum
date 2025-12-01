using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Another_Great_Forum.Pages.Shared
{
    public class CreateForumModel : PageModel
    {

        private readonly HttpClient _httpClient;

        public CreateForumModel(HttpClient client)
        {
            _httpClient = client;
        }

        public class Input
        {
            [Required, MaxLength(200)]
            public string Title { get; set; }

            [Required]
            public string Body { get; set; }

            [Required]
            public int CategoryId { get; set; }

            [Required]
            public string AuthorId { get; set; }

        }

        [BindProperty]
        public Input input { get; set; }

        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        public record CategoryDto(int Id, string Name, string Description);

        public async Task OnGet()
        {
            Categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("https://localhost:7242/categories")
             ?? new List<CategoryDto>();

        }

        public async Task <IActionResult> OnPostAsync()
        {
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7242/posts", input);
            

            if(!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the forum post.");
                return Page();
            }
            
            return RedirectToPage("./Index");
        }
    }
}
