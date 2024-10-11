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
		public string PopupMessage { get; set; } = string.Empty;
		[BindProperty]
		public string CouponCode { get; set; } = string.Empty;

		public string CouponMessage { get; set; } = string.Empty;
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
			var user = await _userBusiness.GetFromCookie(Request);
			if (user == null)
			{
				return RedirectToPage("/Login");
			}
			var result = await _userBusiness.GetById(user.UserId);
			if (result.Data == null || result.Status < 0)
			{
				return RedirectToPage("/Login");
			}
			else
			{
				User1 = (User)result.Data;
			}
			if (await GetOrderInfo(id) == true)
			{
				return Page();
			}
			return RedirectToPage("/Error");
		}


		public async Task<IActionResult> OnPostAsync(string action, int id, int total)
		{
			var user = await _userBusiness.GetFromCookie(Request);
			if (user == null)
			{
				return RedirectToPage("/Login");
			}
			var results = await _userBusiness.GetById(user.UserId);
			if (results.Status < 0 || results.Data == null)
			{
				return RedirectToPage("/Login");
			}
			User1 = (User)results.Data;

			switch (action)
			{
				case "saveDetails":
					User1.FullName = Order1.FullName;
					User1.Address = Order1.Address;
					User1.PhoneNumber = Order1.PhoneNumber;
					var result = await _userBusiness.Update(User1).ConfigureAwait(false);
					if (result.Status > 0)
					{
						PopupMessage = "Thông tin đã được lưu!";
					}
					else
					{
						PopupMessage = "Có lỗi xảy ra!";

					}
					return await OnGetAsync(id);
				case "applyCoupon":
					var coupon = await _couponBusiness.ApplyCouponAsync(CouponCode, total);
					CouponMessage = coupon.message;
					Total = coupon.totalWithDiscount;
					Discount = coupon.discount;
					return await OnGetAsync(id);
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
			if (order1 == null || !(order1.Status > 0) || order1.Data == null)
			{
				return false;

			}
			Order1 = (Order)order1.Data;
			Details = await _orderDetailBusiness.GetDetailsByOrderId(orderId);
			if (Details == null || !(Details.Count > 0))
			{
				return false;
			}
			foreach (var order in Details)
			{
				var item = await _productBusiness.GetById(order.ProductId);
				if (item == null || !(item.Status > 0) || item.Data == null)
				{
					return false;
				}
				var product = (Product)item.Data;
				Products.Add(product);
				if (!CouponMessage.Contains("Coupon applied"))
				{
					Total += product.Price;
				}
			}
			return true;
		}

	}
}

