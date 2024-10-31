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

		public string PopupMessage { get; set; } = string.Empty;
		public async Task<IActionResult> OnGetAsync(string? token)
		{
			if (string.IsNullOrWhiteSpace(token))
			{
				return RedirectToPage("/Error");
			}
			var UserProfile = await _userBusiness.GetFromCookie(Request);
			if (UserProfile == null)
			{
				return RedirectToPage("/Login");
			}
			var user = await _userBusiness.GetUserByHashedToken(token);
			if (user == null || user == UserProfile || string.IsNullOrWhiteSpace(user.Token) || !FormatUtilities.ValidateToken(user.Token))
			{
				return RedirectToPage("/Error");
			}
			return Page();
		}
		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				var user = await _userBusiness.GetFromCookie(Request);
				if (user == null)
				{
					return RedirectToPage("/Login");
				}
				if (user.Email.Trim().Equals(NewEmail.Trim(), StringComparison.CurrentCultureIgnoreCase))
				{
					PopupMessage = "This is your old email.";
					return Page();
				}
				else
				{
					var checkEmail = await _userBusiness.GetByEmailAsync(NewEmail);
					if (checkEmail.Data != null)
					{
						PopupMessage = "This email already exist.";
						return Page();
					}
				}
				await _userBusiness.ChangeEmail(user, NewEmail);
				PopupMessage = "Your email has been updated successfully.Confirm your email and login again to continue";
				Response.Cookies.Delete("User");
				return RedirectToPage("/Index");
			}
			catch (Exception ex)
			{
				PopupMessage = "An error occurred while updating your email: " + ex.Message;
				return Page();
			}
		}
	}
}