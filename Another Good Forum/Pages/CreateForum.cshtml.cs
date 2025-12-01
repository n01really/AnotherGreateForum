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

        public class Input()
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

        //public List<CategoryDto> Categories { get; set; } = new();

        //public async Task OnGetAsync()
        //{
        //    var request = await _httpClient.GetAsync();
        //}



        public async Task <IActionResult> OnPostAsync()
        {
            var client = _httpClient;
            var response = await client.PostAsJsonAsync("https://localhost:7242/posts", input);
            

            if(!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the forum post.");
                return Page();
            }
            
            return RedirectToPage("./Index");
        }
    }
}
