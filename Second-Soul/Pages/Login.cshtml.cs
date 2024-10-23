using BusinessObject.Base;
using BusssinessObject;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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
        public LoginInput InputLogin { get; set; } = new();
        [BindProperty]
        public RegisterInput InputRegister { get; set; } = new();
        public class RegisterInput
        {
            [Required]
            [MaxLength(50)]
            public string Username { get; set; } = string.Empty;
            [Required]
            [MaxLength(50)]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }
        public class LoginInput
        {

            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }
        public async Task<IActionResult> OnPostSignUpAsync()
        {

            var existingUser = await _userBusiness.GetByEmailAsync(InputRegister.Email);
            if (existingUser.Status > 0 && existingUser.Data != null)
            {
                ViewData["ErrorMessage"] = "This email is already registered.";
                return Page();
            }
            var newUser = new User
            {
                Username = InputRegister.Username,
                FullName = InputRegister.FullName,
                PasswordHash = InputRegister.Password,
                Email = InputRegister.Email,
                Role = "Customer",
                ImageUrl = null,
                CreatedDate = DateTime.Now,
                IsActive = false
            };
            var result = await _userBusiness.Register(newUser);

            if (result.Status > 0 && result.Data != null)
            {
                ViewData["SuccessMessage"] = "Registration Success.Please Confirm the email.";
                return Page();
            }
            ViewData["ErrorMessage"] = "Please fill in all required fields correctly.";

            return Page();
        }
        public async Task<IActionResult> OnPostLoginAsync()
        {

            var result = await _userBusiness.GetByEmailOrUserNameAndPasswordAsync(InputLogin.Email, InputLogin.Password);
            if (result != null)
            {
                if (result.Status > 0 && result.Data != null)
                {
                    var user = (User)result.Data;
                    if (user.IsActive == false)
                    {
                        ViewData["ErrorMessage"] = "Please confirm email to login.";
                        return Page();
                    }
                    var userJson = JsonSerializer.Serialize(user);

                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    };

                    Response.Cookies.Append("User", userJson, cookieOptions);
                    return RedirectToPage("/Index");
                }
                else
                {
                    Console.WriteLine(result.Message);
                }
            }
            ViewData["ErrorMessage"] = "Invalid email or password.";
            return Page();
        }
    }
}
