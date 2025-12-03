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
                var response = await _httpClient.GetAsync("/categories");
                if (response.IsSuccessStatusCode)
                {
                    Categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
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
                // Note: You'll need to create a GetUsers endpoint in your API
                // For now, we'll get users from posts
                var response = await _httpClient.GetAsync("/posts");
                if (response.IsSuccessStatusCode)
                {
                    var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>() ?? new();
                    Users = posts
                        .Where(p => p.Author != null)
                        .Select(p => p.Author!)
                        .GroupBy(u => u.Id)
                        .Select(g => g.First())
                        .ToList();
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
                var response = await _httpClient.GetAsync("/posts");
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
