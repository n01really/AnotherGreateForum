using Another_Great_Forum.Data;
using Another_Great_Forum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

            Forums = await _context.Forums.ToListAsync();
           
        }

        public async Task <IActionResult> OnPostAsync()
        {

            Forum.CreatedOnDate = DateTime.UtcNow;

            if (!ModelState.IsValid)
                return Page();

            _context.Forums.Add(Forum);
            await _context.SaveChangesAsync();
           return RedirectToPage("./Index");
        }
    }
}
