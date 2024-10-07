using BusssinessObject;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace Second_Soul.Pages.UserPage
{
	public class ProfileModel : PageModel
	{
		private readonly Cloudinary _cloudinary;
		private readonly UserBusiness _userBusiness;

		public ProfileModel(Cloudinary cloudinary, UserBusiness userBusiness)
		{
			_cloudinary = cloudinary;
			_userBusiness = userBusiness;
		}

		public User? UserProfile { get; set; } = null;

		[BindProperty]
		public string ErrorMessage { get; set; } = string.Empty;
		[BindProperty]
		public string SuccessMessage { get; set; } = string.Empty;

		public IActionResult OnGet()
		{
			if (!TrySetUserFromCookie())
			{
				return RedirectToPage("/Login");
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(IFormFile file)
		{
			if (!TrySetUserFromCookie() || UserProfile == null)
			{
				return RedirectToPage("/Login");
			}

			if (file != null)
			{
				var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
				var fileExtension = Path.GetExtension(file.FileName).ToLower();
				if (!allowedExtensions.Contains(fileExtension))
				{
					ErrorMessage = "Only PNG, JPG, and JPEG formats are allowed.";
					return Page();
				}

				var uploadParams = new ImageUploadParams()
				{
					File = new FileDescription(file.FileName, file.OpenReadStream())
				};

				var uploadResult = await _cloudinary.UploadAsync(uploadParams);
				UserProfile.ImageUrl = uploadResult.SecureUrl.ToString();
				await _userBusiness.Update(UserProfile);
			}
			else
			{
				ErrorMessage = "Please select a file to upload.";
				return Page();
			}

			SuccessMessage = "Profile picture successfully changed.";
			return RedirectToPage();
		}

		private bool TrySetUserFromCookie()
		{
			var result = UserBusiness.GetFromCookie(Request);
			if (result != null)
			{
				if (result.Status > 0 && result.Data != null)
				{
					UserProfile = (User)result.Data;
					return true;

				}
				else
				{
					Console.WriteLine(result.Message);
				}
			}

			return false;
		}
	}
}
