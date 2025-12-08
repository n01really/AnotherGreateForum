using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Another_Great_Forum.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _httpClient;

        // ? Inject HttpClient via constructor
        public RegisterModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public class InputModel
        {
            [Required]
            public string DisplayName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required, MinLength(6)]
            public string Password { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var requestBody = new
            {
                DisplayName = Input.DisplayName,
                Email = Input.Email,
                Password = Input.Password
            };

            var response = await _httpClient.PostAsJsonAsync("/users/register", requestBody);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Registration successful!");
                return RedirectToPage("/Index");
            }
            else
            {
                Console.WriteLine("Registration failed.");
                ModelState.AddModelError(string.Empty, "Registration failed. Try again.");
                return Page();
            }
        }
    }
}