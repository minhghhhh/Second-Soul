using BusssinessObject;
using Data.Models;
using Data.Utils;
using Data.Utils.HashPass;
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Second_Soul.Pages
{
    public class ResetModel : PageModel
    {
        private readonly IUserBusiness _userBusiness;
        public ResetModel(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        [BindProperty]
        public ResetInput Input { get; set; } = new ResetInput();

        public void OnGet()
        {
            ViewData["TokenError"] = TempData["TokenError"];
        }
        public async Task<IActionResult> OnPost(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                ViewData["TokenError"] = "Email is required.";
                return Page();
            }
            var userResult = await _userBusiness.GetByEmailAsync(Email);
            if (userResult == null || !(userResult.Status > 0) || userResult.Data == null)
            {   
                ViewData["SuccessMessage"] = "If an account with that email exists, a password reset token will be sent.";
                return Page();
            }

            var user = (User)userResult.Data;
            user.Token = FormatUtilities.GenerateRandomCodeWithExpiration(20);

            var result = await _userBusiness.Update(user);

            if (result != null && result.Status > 0)
            {
				var resetLink =
					$"{Request.Scheme}://{Request.Host}/Verify?token={HashPassWithSHA256.HashWithSHA256(user.Token)}";
				bool emailSent = await SendMail.SendResetLinkEmail(user.Email, resetLink);
                if (emailSent)
                {
					ViewData["SuccessMessage"] = "A link has been sent to your email.";
					return Page();
                }
            }
			ViewData["TokenError"] = "Failed to send email. Please try again.";
            return Page();
        }
        public class ResetInput
        {
            [Required(ErrorMessage = "Token is required.")]
            public string Token { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "New password is required.")]
            [DataType(DataType.Password)]
            [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[A-Za-z\\d]{8,}$",
                ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, and one number.")]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please confirm your new password.")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
        public static string GenerateTokenAsync()
        {
            // Generate a secure random number
            byte[] randomNumber = new byte[4]; // Generates a 4-byte (32-bit) number
            RandomNumberGenerator _rng = RandomNumberGenerator.Create();
            _rng.GetBytes(randomNumber);
            string token = BitConverter.ToUInt32(randomNumber, 0).ToString().PadLeft(10, '0'); // 10-digit number
                                                                                               // Store token logic here
            return token;
        }
    }
}
