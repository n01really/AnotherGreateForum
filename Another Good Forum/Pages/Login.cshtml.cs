using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace Another_Great_Forum.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        // Use a CookieContainer to store cookies
        private readonly CookieContainer _cookieContainer;

        public LoginModel()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                UseCookies = true
            };
            _httpClient = new HttpClient(handler);
        }

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var requestBody = new
            {
                UserName = Input.Email,
                Password = Input.Password
            };

            // Send login request
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7242/users/login", requestBody);

            if (response.IsSuccessStatusCode)
            {
                // Get cookies set by the API
                var cookies = _cookieContainer.GetCookies(new Uri("https://localhost:7242"));

                foreach (Cookie cookie in cookies)
                {
                    // Optional: log cookie names for debugging
                    Console.WriteLine($"{cookie.Name} = {cookie.Value}");
                }

                // Now the cookie is stored in _cookieContainer
                // Future requests using _httpClient will include the cookie automatically
                return RedirectToPage("/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
    }
}
