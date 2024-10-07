using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObject.Base;
using BusssinessObject;
using Data.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                    return Page();
                }

                var newUser = new User
                {
                    Username = Input.Username,
                    PasswordHash = HashPassword(Input.Password),  // Hash the password
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    Address = Input.Address,
                    Role = "Customer",  // Default role
                    CreatedDate = DateTime.Now,  // Set to current date
                    IsActive = true  // Default to active
                };

                var result = await _userBusiness.CreateUserAsync(newUser);
                if (result.Status > 0 && result.Data != null)
                {
                    return RedirectToPage("/Login");
                }
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
            }

            return Page();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
