using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages
{
    public class ContactModel : PageModel
    {
		private readonly UserBusiness _userBusiness;
		public User User { get; set; }

		public ContactModel(UserBusiness userBusiness)
		{
			_userBusiness = userBusiness;
		}

        public async Task OnGet()
        {
			var user = await _userBusiness.GetFromCookie(Request);
			if (user != null)
			{
				
			}
		}

		public void OnPost()
        {

        }
    }
}
