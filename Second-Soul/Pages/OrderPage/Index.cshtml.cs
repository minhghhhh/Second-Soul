using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Net.payOS.Types;
using Net.payOS;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MailKit.Search;
using System.Runtime.Intrinsics.X86;

namespace Second_Soul.Pages.OrderPage
{
    public class IndexModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IPaymentBusiness _paymentBusiness;
        private readonly IProductBusiness _productBusiness;
        private readonly ICouponBusiness _couponBusiness;
        public IndexModel(IOrderBusiness orderBusiness, ICouponBusiness couponBusiness, IOrderDetailBusiness orderDetailBusiness, IUserBusiness userBusiness, IPaymentBusiness paymentBusiness, IProductBusiness productBusiness)
        {
            _couponBusiness = couponBusiness;
            _orderBusiness = orderBusiness;
            _orderDetailBusiness = orderDetailBusiness;
            _userBusiness = userBusiness;
            _paymentBusiness = paymentBusiness;
            _productBusiness = productBusiness;
        }
        public string PopupMessage {  get; set; } 
        [BindProperty]
        public string CouponCode { get; set; }

        public string CouponMessage { get; set; }
        public int Discount { get; set; } = 0;

        [BindProperty]
        public int Total { get; set; } = 0;
        [BindProperty]
        public Order Order1 { get; set; } = new Order();
        public List<OrderDetail> Details { get; set; } = new List<OrderDetail>();
        public List<Product> Products { get; set; } = new List<Product>();
        public User User1 { get; set; } = new User();
        public async Task<IActionResult> OnGetAsync(int id)
        {
            User1 = await _userBusiness.GetFromCookie(Request);
            if (User1 == null)
            {
                return RedirectToPage("/Login");
            }
            if (await GetOrderInfo(id) == true)
            {
                return Page();
            }
            return RedirectToPage("/Error");
        }
     

        public async Task<IActionResult> OnPostAsync(string action)
        {
            User1 = await _userBusiness.GetFromCookie(Request);
            if (User1 == null)
            {
                return RedirectToPage("/Login");
            }

            switch (action)
            {
                case "saveDetails":
                    User1.FullName = Order1.FullName;
                    User1.Address = Order1.Address;
                    User1.PhoneNumber = Order1.PhoneNumber;
                  var result =  await _userBusiness.Update(User1).ConfigureAwait(false);
                    if(result.Status>0)
                    {
                        PopupMessage = "Thông tin đã được lưu!";

                    }
                    else
                    {
                        PopupMessage = "Có lỗi xảy ra!";

                    }
                    return await OnGetAsync(Order1.OrderId);
                case "applyCoupon":
                    // Handle applying coupon
                    break;
                case "placeOrder":
                    // Handle placing the order
                    break;
                case "removeProduct_":
                    {
                        var productId = int.Parse(action.Split('_')[1]);
                    }
                    break;
            }
            return RedirectToPage(); // Reload the page after the action
        }

        public async Task<bool> GetOrderInfo(int orderId)
        {
            
            var order1 = await _orderBusiness.GetById(orderId);
            Order1 = order1.Data as Order;
            if (Order1 == null)
            {
                return false;
            }
            Details = await _orderDetailBusiness.GetDetailsByOrderId(orderId);
            if (Details != null)
            {
                foreach (var order in Details)
                {
                    var item = await _productBusiness.GetById(order.ProductId);
                    var product = item.Data as Product;
                    Products.Add(product);
                    Total += product.Price;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

    }
}

