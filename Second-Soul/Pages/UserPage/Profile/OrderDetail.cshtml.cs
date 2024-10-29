using BusssinessObject;
using CloudinaryDotNet;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.UserPage.Profile
{
    public class OrderDetailModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        private readonly IProductBusiness _productBusiness;


        public OrderDetailModel(IOrderBusiness orderBusiness, IUserBusiness userBusiness, IOrderDetailBusiness orderDetailBusiness,IProductBusiness productBusiness)
        {
            _productBusiness = productBusiness;
            _orderBusiness = orderBusiness;
            _userBusiness = userBusiness;
            _orderDetailBusiness = orderDetailBusiness;
        }
        [BindProperty]
        public string ErrorMessage { get; set; }
        public IList<OrderDetail> Details { get; set; } = new List<OrderDetail>();
        public Order Order { get; set; } 
        public async Task<IActionResult> OnGet(int id)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            else
            {
                var billDetails = await _orderDetailBusiness.GetDetailsByOrderId(id);
                var result = await _orderBusiness.GetById(id);
                Order = (Order)result.Data;
                if (billDetails == null)
                {
                    ErrorMessage = "Bill details not found";
                    return RedirectToPage("/UserPage/Profile/Order");
                }
                if (billDetails[0].Order.CustomerId != user.UserId)
                {
                    ErrorMessage = "You dont have permission to view this bill";
                    return RedirectToPage("/UserPage/Profile/Order");
                }
                foreach (var item in billDetails)
                {
                    if (item != null)
                    {
                        var product = await _productBusiness.GetById(item.ProductId);
                        item.Product = (Product)product.Data;
                        Details.Add(item);
                    }
                }
                return Page();
            }
        }

    }
}
