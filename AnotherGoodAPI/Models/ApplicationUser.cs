using Microsoft.AspNetCore.Identity;

namespace AnotherGoodAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
        public ICollection<DirectMessage> SentMessages { get; set; } = new List<DirectMessage>();
        public ICollection<DirectMessage> ReceivedMessages { get; set; } = new List<DirectMessage>();
    }
}
