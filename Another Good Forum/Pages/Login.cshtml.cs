using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Another_Great_Forum.Pages
{
    public class LoginModel : PageModel
    {
        // No server-side HttpClient needed for login
        public void OnGet()
        {
        }
    }
}
