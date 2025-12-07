using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Another_Great_Forum.Pages
{
    public class AdminPageModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminPageModel> _logger;

        public AdminPageModel(IHttpClientFactory httpClientFactory, ILogger<AdminPageModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public bool IsAdmin { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<CategoryDto> Categories { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();
        public List<PostDto> Posts { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // TODO: Remove this temporary bypass once authentication is implemented
            IsAdmin = true;
            UserName = "Test Admin";

            // Fetch data from API
            await LoadCategoriesAsync();
            await LoadUsersAsync();
            await LoadPostsAsync();

            return Page();

            /* Original authentication code - restore when ready:
            try
            {
                var authCookie = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
                
                if (string.IsNullOrEmpty(authCookie))
                {
                    return RedirectToPage("/Login");
                }

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

                if (!userData.Roles.Contains("Admin"))
                {
                    ErrorMessage = "Access denied. You must be an administrator to view this page.";
                    IsAdmin = false;
                    return Page();
                }

                IsAdmin = true;
                
                // Fetch data from API
                await LoadCategoriesAsync();
                await LoadUsersAsync();
                await LoadPostsAsync();
                
                return Page();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error connecting to API");
                ErrorMessage = "Unable to verify admin status. Please try again later.";
                return Page();
            }
            */  
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(nameof(AdminPageModel));
                _logger.LogInformation($"HttpClient BaseAddress: {httpClient.BaseAddress?.ToString() ?? "NULL"}");
                _logger.LogInformation($"Attempting to call: {httpClient.BaseAddress}categories");
                
                var response = await httpClient.GetAsync("/categories");
                if (response.IsSuccessStatusCode)
                {
                    Categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
                    _logger.LogInformation($"Loaded {Categories.Count} categories");
                }
                else
                {
                    _logger.LogWarning($"Failed to load categories. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(nameof(AdminPageModel));
                var response = await httpClient.GetAsync("/users");
                if (response.IsSuccessStatusCode)
                {
                    Users = await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? new();
                    _logger.LogInformation($"Loaded {Users.Count} users");
                }
                else
                {
                    _logger.LogWarning($"Failed to load users. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
            }
        }

        private async Task LoadPostsAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(nameof(AdminPageModel));
                var response = await httpClient.GetAsync("/posts");
                if (response.IsSuccessStatusCode)
                {
                    Posts = await response.Content.ReadFromJsonAsync<List<PostDto>>() ?? new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading posts");
            }
        }

        private record UserResponse(string Id, string DisplayName, string Email, List<string> Roles);

        public record CategoryDto(int Id, string Name, string Description);

        public record UserDto(string Id, string DisplayName, string? Email, string? ProfilePictureUrl);

        public record PostDto(
            int Id,
            string Title,
            string Body,
            DateTime CreatedAt,
            DateTime? UpdatedAt,
            string AuthorId,
            UserDto? Author,
            int CategoryId,
            CategoryDto? Category,
            List<CommentDto>? Comments,
            List<LikeDto>? Likes
        );

        public record CommentDto(int Id, string Body, DateTime CreatedAt);
        public record LikeDto(string UserId, int PostId);
    }
}
