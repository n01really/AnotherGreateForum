using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;


namespace Another_Great_Forum.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LoginModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public class InputModel {

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
                
        };

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var requestBody = new
            {
                UserName = Input.Email,
                Password = Input.Password
            };

            var response = await _httpClient.PostAsJsonAsync("/users/login", requestBody);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                // Login failed, show error message
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
    }
}
