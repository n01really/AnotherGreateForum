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
            // Temporary admin bypass
            IsAdmin = true;
            UserName = "Test Admin";

            // Load data
            await LoadCategoriesAsync();
            await LoadUsersAsync();
            await LoadPostsAsync();

            return Page();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(nameof(AdminPageModel));
                var response = await httpClient.GetAsync("/categories");
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
                var httpClient = _httpClientFactory.CreateClient(nameof(AdminPageModel));
                var response = await httpClient.GetAsync("/users");
                if (response.IsSuccessStatusCode)
                {
                    Users = await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? new();
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

        public async Task<IActionResult> OnPostCreateCategoryAsync(string Name, string Description)
        {
            var httpClient = _httpClientFactory.CreateClient(nameof(AdminPageModel));
            var body = new { Name, Description };
            await httpClient.PostAsJsonAsync("/categories", body);
            await LoadCategoriesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteCategoryAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient(nameof(AdminPageModel));
            await httpClient.DeleteAsync($"/categories/{id}");
            await LoadCategoriesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePostAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient(nameof(AdminPageModel));
            var response = await httpClient.DeleteAsync($"/posts/{id}");
            
            _logger.LogWarning($"Delete post {id} response: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to delete post {id}. Status: {response.StatusCode}, Content: {errorContent}");
                ErrorMessage = $"Failed to delete post. Status: {response.StatusCode}";
            }
            
            // Reload all data to ensure proper relationships
            await LoadCategoriesAsync();
            await LoadUsersAsync();
            await LoadPostsAsync();
            
            return RedirectToPage();
        }

        // DTOs
        private record UserResponse(string Id, string DisplayName, string Email, List<string> Roles);

        public record CategoryDto(int Id, string Name, string Description);

        public record UserDto(string Id, string DisplayName, string? Email, string? ProfilePictureUrl);

        public record PostDto(
            int Id,
            string Title,
            string Body,
            string AuthorName,
            string CategoryName,
            DateTime CreatedAt,
            int CommentCount,
            int LikeCount
        );

        public record CommentDto(int Id, string Body, DateTime CreatedAt);
        public record LikeDto(string UserId, int PostId);
    }
}
