using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Another_Great_Forum.Pages
{
    public class AdminPageModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AdminPageModel> _logger;

        public AdminPageModel(HttpClient httpClient, ILogger<AdminPageModel> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public bool IsAdmin { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get the authentication cookie from the current request
                var authCookie = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
                
                if (string.IsNullOrEmpty(authCookie))
                {
                    return RedirectToPage("/Login");
                }

                // Forward the cookie to the API
                var request = new HttpRequestMessage(HttpMethod.Get, "/users/current");
                request.Headers.Add("Cookie", $".AspNetCore.Identity.Application={authCookie}");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Login");
                }

                var userData = await response.Content.ReadFromJsonAsync<UserResponse>();
                
                if (userData == null)
                {
                    return RedirectToPage("/Login");
                }

                UserName = userData.DisplayName;

                // Check if user has Admin role
                if (!userData.Roles.Contains("Admin"))
                {
                    ErrorMessage = "Access denied. You must be an administrator to view this page.";
                    IsAdmin = false;
                    return Page();
                }

                IsAdmin = true;
                return Page();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error connecting to API");
                ErrorMessage = "Unable to verify admin status. Please try again later.";
                return Page();
            }
        }

        private record UserResponse(string Id, string DisplayName, string Email, List<string> Roles);
    }
}
