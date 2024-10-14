using BusssinessObject;
using Data.Models;
using Data.Utils;
using Data.Utils.HashPass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Second_Soul.Pages
{
	public class VerifyModel : PageModel
	{
		private readonly IUserBusiness _userBusiness;

		public VerifyModel(IUserBusiness userBusiness)
		{
			_userBusiness = userBusiness;
		}
		[BindProperty]
		public ResetInput Input { get; set; } = new ResetInput();

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


		public async Task<IActionResult> OnGetAsync(string? token)
		{
			if (!string.IsNullOrEmpty(token))
			{
				var user = await _userBusiness.GetUserByHashedToken(token);
				if (user != null && !string.IsNullOrEmpty(user.Token))
				{
					if (FormatUtilities.ValidateToken(user.Token))
					{
						Input.Email = user.Email;
						Input.Token = user.Token;
						return Page();
					}
				}
			}
			TempData["TokenError"] = "The URL doesn't work. Please try again.";
			return RedirectToPage("/Reset");
		}

		public async Task<IActionResult> OnPostConfirmToken()
		{
			if (!ModelState.IsValid)
			{
				return await OnGetAsync(Input.Token);
			}

			if (Input.NewPassword != Input.ConfirmPassword)
			{
				ModelState.AddModelError("Input.ConfirmPassword", "The password and confirmation password do not match.");
				return await OnGetAsync(Input.Token);
			}

			var userResult = await _userBusiness.GetByEmailAsync(Input.Email);
			if (userResult == null || !(userResult.Status > 0) || userResult.Data == null)
			{
				TempData["TokenError"] = "The account in need of a password reset cannot be identified. Please try again.";
				return RedirectToPage("/Reset");
			}
			var user = (User)userResult.Data;

			user.PasswordHash = HashPassWithSHA256.HashWithSHA256(Input.NewPassword);
			var result = await _userBusiness.Update(user);
			if (result == null || !(result.Status > 0))
			{
				ModelState.AddModelError("", "Resetting the password has failed. Try again at a later time.");
				return await OnGetAsync(Input.Token);
			}
			return RedirectToPage("/Login");
		}

	}
}
