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
		public LoginInput Input { get; set; } = new();

		public class LoginInput
		{
			public LoginInput()
			{
			}

			[Required]
			public string Email { get; set; } = string.Empty;

			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; } = string.Empty;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var result = await _userBusiness.GetByEmailOrUserNameAndPasswordAsync(Input.Email, Input.Password);
                if (result != null)
				{
					if (result.Status > 0 && result.Data != null)
                    {
                        var user = (User)result.Data;
                        if (user.IsActive == false)
                        {
                            ModelState.AddModelError(string.Empty, "Please confirm email to login.");
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
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			}
			return Page();
		}
	}
}
