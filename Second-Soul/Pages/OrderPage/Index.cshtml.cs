using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.OrderPage
{
    public class IndexModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IPaymentBusiness _paymentBusiness;
        public IndexModel(IOrderBusiness orderBusiness, IOrderDetailBusiness orderDetailBusiness, IUserBusiness userBusiness, IPaymentBusiness paymentBusiness)
        {
            _orderBusiness = orderBusiness;
            _orderDetailBusiness = orderDetailBusiness;
            _userBusiness = userBusiness;
            _paymentBusiness = paymentBusiness;
        }
        public User User { get; set; }
        [BindProperty]
        public Order Order { get; set; }
        [BindProperty]
        public List<OrderDetail> Details { get; set; }
        public List<ShoppingCart> CartItems { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            User = await _userBusiness.GetFromCookie(Request);
            if (User == null)
            {
                return RedirectToPage("/Login");
            }
            Details = new List<OrderDetail>();
            return Page();
        }
    }
}
