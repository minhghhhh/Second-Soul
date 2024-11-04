using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.OrderPage
{
	public class CancelPaymentModel : PageModel
	{
		private readonly IUserBusiness _userBusiness;
		private readonly IPaymentBusiness _paymentBusiness;
		private readonly IOrderBusiness _orderBusiness;
		public CancelPaymentModel(IPaymentBusiness paymentBusiness, IOrderBusiness orderBusiness,IUserBusiness userBusiness)
		{
			_userBusiness = userBusiness;
			_paymentBusiness = paymentBusiness;
			_orderBusiness = orderBusiness;
		}
		public async Task<IActionResult> OnGet(string? token)
		{
            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToPage("/Index");
            }
            var user = await _userBusiness.GetUserByToken(token);
            if (user == null)
            {
                return RedirectToPage("/Index");
            }
            var result = await _orderBusiness.GetSinglePendingOrder(user.UserId);
            if (result == null || !(result.Status > 0) || result.Data == null)
            {
                return RedirectToPage("/Index");
            }
            var order = (Order)result.Data;
            await _paymentBusiness.CancelOrder(order.OrderId);
            return Page();
		}
	}
}
