using BusssinessObject;
using Data.Models;
using Data.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages
{
    public class FailModel : PageModel
    {
        private readonly IUserBusiness _userBusiness;
        public FailModel(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }
        public async void OnGet(int id)
        {
            var var = await _userBusiness.GetById(id);
            var user = (User)var.Data;
            await SendMail.SendWrongInformationEmail(user.Email, user);
        }
    }
}
