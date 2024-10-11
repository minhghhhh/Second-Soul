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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            /*            User1 = await _userBusiness.GetFromCookie(Request);
                        if (User1 == null)
                        {
                            return RedirectToPage("/Login");
                        }
                        var TotalShip = Total + 30000;
                        var order = new Order1
                            {


                            };
                        ItemData item = new ItemData("Mì tôm hảo hảo ly", 1, 1000);
                        List<ItemData> items = new List<ItemData>();
                        items.Add(item);
                        PaymentData paymentData = new PaymentData(orderCode, TotalShip, "Thanh toan don hang",
                             items, cancelUrl = "https://localhost:7141", returnUrl = "https://localhost:7141");

                        CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

            */
            return Page();
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

