using Another_Great_Forum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Another_Great_Forum.Pages
{
    public class DetailsModel : PageModel
    {

        private readonly Data.ApplicationDbContext _context;

        public DetailsModel(Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Forum Forum { get; set; }

        //public Task<IActionResult> OnGetAsync()
        //{
        //    Forum = _context.Forums.Find();
        //}
    }
}
