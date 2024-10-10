using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Second_Soul.Pages.OrderPage
{
    public class IndexModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IPaymentBusiness _paymentBusiness;
        private readonly IProductBusiness _productBusiness;
        public IndexModel(IOrderBusiness orderBusiness,  IOrderDetailBusiness orderDetailBusiness, IUserBusiness userBusiness, IPaymentBusiness paymentBusiness, IProductBusiness productBusiness)
        {
            _orderBusiness = orderBusiness;
            _orderDetailBusiness = orderDetailBusiness;
            _userBusiness = userBusiness;
            _paymentBusiness = paymentBusiness;
            _productBusiness = productBusiness;
        }
        [BindProperty]
        public int Total { get; set; } = 0;
        public Order Order { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public User User { get; set; }
        public List<int> SelectedProductIds { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            User = await _userBusiness.GetFromCookie(Request);
            if (User == null)
            {
                return RedirectToPage("/Login");
            }
            if (TempData["SelectedProductIds"] != null)
            {   
                SelectedProductIds = JsonSerializer.Deserialize<List<int>>(TempData["SelectedProductIds"] as string);
            }
            if (SelectedProductIds != null)
            {
                foreach (var id in SelectedProductIds)
                {
                    var item = await _productBusiness.GetById(id);
                    var product = item.Data as Product;
                    Products.Add(product);
                    Total += product.Price;
                }
            }
            else
            {
                return RedirectToPage("/Error");
            }
            return Page();
        }
    }
}
