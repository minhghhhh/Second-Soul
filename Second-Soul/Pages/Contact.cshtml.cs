using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages
{
    public class ContactModel : PageModel
    {
		private readonly IUserBusiness _userBusiness;
		public User? User1 { get; set; }

		public ContactModel(IUserBusiness userBusiness)
		{
			_userBusiness = userBusiness;
		}

        public async Task OnGet()
        {
			User1 = await _userBusiness.GetFromCookie(Request);
			
		}

		public void OnPost()
        {

        }
    }
}
