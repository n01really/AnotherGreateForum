using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Another_Great_Forum.Pages
{
    public class RegisterModel : PageModel
    {
        public void OnGet()
        {
            var requestBody = new {displayName = Input.DisplayName, email = Input.Email, password = Input.Password};
            var response = await _httpClient.PostAsJsonAsync("/users/register", requestBody);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Registration successful!");
                return RedirectToPage("./Index");
            }
            else
            {
                Console.WriteLine("Registration failed..");
                return Page();
            }

            
        }
    }
}