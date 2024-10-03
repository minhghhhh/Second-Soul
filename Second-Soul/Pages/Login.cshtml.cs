using BusinessObject.Base;
using BusssinessObject;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Second_Soul.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserBusiness _userBusiness;
        public LoginModel(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        [BindProperty]
        public LoginInput Input { get; set; }

        public class LoginInput
        {
            [Required]
            [EmailAddress]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _userBusiness.GetByEmailAndPasswordAsync(Input.UserName, Input.Password);
                if (result != null && result.Status > 0 && result.Data != null)
                {
                    var user = result.Data as User;
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return Page();
        }
    }
}
