using BusssinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.OrderPage
{
	public class CancelPaymentModel : PageModel
	{
		public int Id { get; set; } = 0;
		private readonly IPaymentBusiness _paymentBusiness;
		private readonly IOrderBusiness _orderBusiness;
		public CancelPaymentModel(IPaymentBusiness paymentBusiness, IOrderBusiness orderBusiness)
		{
			_paymentBusiness = paymentBusiness;
			_orderBusiness = orderBusiness;
		}
		public async Task OnGet(int id)
		{
			Id = id;
			var result = await _orderBusiness.ReadOnlyById(Id);
			if (result == null || !(result.Status > 0) || result.Data == null)
			{
				RedirectToPage("/Index");
			}
			await _paymentBusiness.CancelOrder(Id);
		}
	}
}
