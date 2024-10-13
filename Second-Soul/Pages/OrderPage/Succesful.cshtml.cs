using BusssinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.OrderPage
{
    public class SuccessfulModel : PageModel
    {
        private readonly IPaymentBusiness _paymentBusiness;

        public SuccessfulModel(IPaymentBusiness paymentBusiness)
        {
            _paymentBusiness = paymentBusiness;

        }
        public void OnGet()
        {
            
        }
    }
}
