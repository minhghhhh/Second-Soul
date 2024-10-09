using BusssinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages
{
    public class ConfirmModel : PageModel
    {
        private readonly TimeSpan TokenValidityDuration = TimeSpan.FromMinutes(15);
        private readonly IUserBusiness _userBusiness;

        public ConfirmModel(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync(string token)
        {


            // Validate the token
            var user = await _userBusiness.GetUserByToken(token);
            if (user == null)
            {
                Message = "Invalid token.";
                return Page();
            }
            if (user.IsActive == true)
            {
                Message = "Email already confirm.";
                return Page();
            }
            if (user.CreatedDate == default || DateTime.UtcNow - user.CreatedDate > TokenValidityDuration)
            {
                Message = "The confirmation token has expired. Please register again.";
                await _userBusiness.DeleteById(user.UserId);
                return Page();
            }
            user.IsActive = true;
            await _userBusiness.Update(user);

            Message = "Email confirmed successfully,Login to continues!";
            return Page();
        }
    }
}
