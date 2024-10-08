using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Second_Soul.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserBusiness _userBusiness;
        public RegisterModel(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }
        [BindProperty]
        public RegisterInput Input { get; set; } = new();
        public class RegisterInput
        {
            [Required]
            [MaxLength(50)]
            public string Username { get; set; } = string.Empty;
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
            [MaxLength(15)]
            public string? PhoneNumber { get; set; }
            [MaxLength(255)]
            public string? Address { get; set; }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userBusiness.GetByEmailAsync(Input.Email);
                if (existingUser.Status > 0 && existingUser.Data != null)
                {
                    ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                    return Page();
                }
                var newUser = new User
                {
                    Username = Input.Username,
                    PasswordHash = Input.Password,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    Address = Input.Address,
                    Role = "Customer",
                    ImageUrl = null,
                    CreatedDate = DateTime.Now,  
                    IsActive = false 
                };
                var result = await _userBusiness.Register(newUser);

                if (result.Status > 0 && result.Data != null)
                {
                    ModelState.AddModelError(string.Empty, "Registration Success.Please Confirm the email");
                    return RedirectToPage("/Login");
                }
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");


            }
            return Page();
        }
    }
}
