using BusssinessObject;
using Data.Models;
using Data.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages
{
    public class ContactModel : PageModel
    {
		private readonly IUserBusiness _userBusiness;
		public User? User1 { get; set; }

		public string PopUpMessage { get; set; } = string.Empty;

		public ContactModel(IUserBusiness userBusiness)
		{
			_userBusiness = userBusiness;
		}

        public async Task OnGet()
        {
			User1 = await _userBusiness.GetFromCookie(Request);
			
		}

		public async Task<IActionResult> OnPost(string name, string email, string message)
        {
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
			{
				return Page();
			}
			await SendMail.SendContactMail(FormatUtilities.TrimSpacesPreserveSingle(name), email, FormatUtilities.TrimSpacesPreserveSingle(message));
			PopUpMessage = "Your message has beent sent.";
			return Page();
        }
    }
}
