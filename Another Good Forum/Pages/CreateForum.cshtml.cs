using Another_Great_Forum.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Another_Great_Forum.Pages.Shared
{
    public class CreateForumModel : PageModel
    {

        private readonly Data.ApplicationDbContext _context;

        public CreateForumModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Forum Forum { get; set; }

        public List<Models.Forum> Forums { get; set; }


        public async Task OnGetAsync()
        {
           
        }

        public async Task <IActionResult> OnPostAsync()
        {
            

           return RedirectToPage("./Index");
        }
    }
}
