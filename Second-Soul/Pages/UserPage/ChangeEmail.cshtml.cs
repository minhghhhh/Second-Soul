using BusssinessObject;
using Data.Models;
using Data.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Second_Soul.Pages.UserPage
{
    public class ChangeEmailModel : PageModel
    {
        private readonly IUserBusiness _userBusiness;
        public ChangeEmailModel(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string NewEmail { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; } = string.Empty;
        [BindProperty]
        public string SuccessMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var UserProfile = await _userBusiness.GetFromCookie(Request);
            if (UserProfile == null)
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            try
            {
                if (user.Email.Trim().ToLower() == NewEmail.Trim().ToLower())
                {
                    ErrorMessage = "This is your old email.";
                    return Page();
                }
                else
                {
                    var checkEmail = await _userBusiness.GetByEmailAsync(NewEmail);
                    if (checkEmail.Data != null)
                    {
                        ErrorMessage = "This email is already exist.";
                        return Page();
                    }
                }
                await _userBusiness.ChangeEmail(user, NewEmail);
                SuccessMessage = "Your email has been updated successfully.Confirm your email and login again to continue";
                    await Task.Delay(2000);
                Response.Cookies.Delete("User");
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while updating your email. Please try again.";
                return Page();
            }
        }
    }
}