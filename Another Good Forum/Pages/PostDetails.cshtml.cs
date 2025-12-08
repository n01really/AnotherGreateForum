using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Another_Great_Forum.Pages
{
    public class PostDetailsModel : PageModel
    {

        public List<PostDto> Posts { get; set; } = new List<PostDto>();
        public record PostDto(int Id, string Title, string Body, string AuthorName, string CategoryName, DateTime CreatedAt, int CommentCount, int LikeCount);
        public void OnGet()
        {
        }
    }
}
