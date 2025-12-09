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
                    var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>() ?? new();

                    // Assign Author and Category using 'with' expression
                    Posts = posts.Select(p => p with
                    {
                        Author = Users.FirstOrDefault(u => u.Id == p.AuthorId),
                        Category = Categories.FirstOrDefault(c => c.Id == p.CategoryId)
                    }).ToList();
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
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to delete post.";
            }
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
