using BusssinessObject;
using CloudinaryDotNet;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Second_Soul.Pages.UserPage.Profile
{
    public class OrderModel : PageModel
    {
        private readonly IOrderBusiness _orderBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;


        public OrderModel(IOrderBusiness orderBusiness, IUserBusiness userBusiness, IOrderDetailBusiness orderDetailBusiness)
        {
            _orderBusiness = orderBusiness;
            _userBusiness = userBusiness;
            _orderDetailBusiness = orderDetailBusiness;
        }
        [BindProperty]
        public string ErrorMessage { get; set; }
        [BindProperty]
        public string SuccessMessage { get; set; }
        public Order order { get; set; }
        public List<Order> Orders { get; set; }
        public List<OrderDetail> Details { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            Orders = await _orderBusiness.GetOrdersByUserId(user.UserId);
            if (Orders.Count == 0)
            {
                ErrorMessage = "You dont have any order";
            }
            return Page();
        }
        public async Task<IActionResult> OnPostSortDate(DateTime fromDate, DateTime toDate)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user != null)
            {
                if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
                {
                    Orders = await _orderBusiness.GetFilterdAccountOrder(fromDate, toDate, user.UserId);
                    if (Orders.Count > 0)
                    {
                        return Page();
                    }
                    else
                    {
                        ErrorMessage = $"No Order's from {fromDate} to {toDate}";
                        return Page();
                    }
                }
                else
                {
                    Orders = await _orderBusiness.GetOrdersByUserId(user.UserId);
                }
                return Page();
            }
            else
            {
                return NotFound();
            }
        }

    }
}
