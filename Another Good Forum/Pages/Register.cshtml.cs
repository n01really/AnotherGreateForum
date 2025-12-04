using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;


namespace Another_Great_Forum.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public RegisterModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public class InputModel
        {
            [Required]
            public string DisplayName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task <IActionResult> OnPostAsync()
        {
            var requestBody = new {displayName = Input.DisplayName, email = Input.Email, password = Input.Password};
            var response = await _httpClient.PostAsJsonAsync("/users/register", requestBody);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Registration successful!");
                return RedirectToPage("./Index");
            }
            else
            {
                Console.WriteLine("Registration failed..");
                return Page();
            }

            
        }
    }
}
