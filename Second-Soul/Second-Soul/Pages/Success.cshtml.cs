using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Utils;

namespace Second_Soul.Pages
{
    public class SuccessModel : PageModel
    {
        private readonly IUserBusiness _userBusiness;
        public SuccessModel(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }
        public async Task OnGet(int id)
        {
                var var = await _userBusiness.GetById(id);
                var User = (User)var.Data;
            if (User != null)
            {
                var wallet = User.Wallet;
                User.Wallet = 0;
                var result = await _userBusiness.Update(User);
                if (result.Status > 0)
                {
                    await SendMail.SendSuccessWithdrawEmail(User.Email,User, wallet);
                }
            }
        }
    }
}
