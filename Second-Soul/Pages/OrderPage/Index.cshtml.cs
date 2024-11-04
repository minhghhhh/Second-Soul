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
using static Azure.Core.HttpHeader;

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
		private readonly IShoppingCartBusiness _shoppingCartBusiness;
		public IndexModel(IOrderBusiness orderBusiness, ICouponBusiness couponBusiness, IOrderDetailBusiness orderDetailBusiness, IUserBusiness userBusiness, IPaymentBusiness paymentBusiness, IProductBusiness productBusiness, IShoppingCartBusiness shoppingCartBusiness)
		{
			_couponBusiness = couponBusiness;
			_orderBusiness = orderBusiness;
			_orderDetailBusiness = orderDetailBusiness;
			_userBusiness = userBusiness;
			_paymentBusiness = paymentBusiness;
			_productBusiness = productBusiness;
			_shoppingCartBusiness = shoppingCartBusiness;
		}
		public string PopupMessage { get; set; } = string.Empty;
		[BindProperty]
		public string CouponCode { get; set; } = string.Empty;

		public string CouponMessage { get; set; } = string.Empty;
		public int Discount { get; set; } = 0;
		public int Total { get; set; } = 0;
		[BindProperty]
		public Order Order1 { get; set; } = new();
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
			var check = false;
			if (User1.Role == "Admin")
			{
				check = await GetOrderInfo(id, null);
			}
			else
			{
				check = await GetOrderInfo(id, User1.UserId);
			}
			if (check)
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
					var coupon = await _couponBusiness.ApplyCouponAsync(CouponCode, id);
					CouponMessage = coupon.message;
					Discount = coupon.discount;
					return await OnGetAsync(id);
				case "placeOrder":
					// Handle placing the order
					var order1 = await _orderBusiness.GetPendingOrder(id, User1.UserId);
					if (order1 == null || !(order1.Status > 0) || order1.Data == null)
					{
						PopupMessage = order1 == null || string.IsNullOrWhiteSpace(order1.Message) ? "The order's retrieval process has encountered an unexpected error." : order1.Message;
						return await OnGetAsync(id);
					}
					if (string.IsNullOrWhiteSpace(Order1.Address) || string.IsNullOrWhiteSpace(Order1.FullName) || string.IsNullOrWhiteSpace(Order1.PhoneNumber))
					{
						PopupMessage =  "The address, full name or phone number is invalid.";
						return await OnGetAsync(id);
					}
					var Order = (Order)order1.Data;
					Order.Address = Order1.Address;
					Order.FullName = Order1.FullName;
					Order.PhoneNumber = Order1.PhoneNumber;
					Order.Descriptions = Order1.Descriptions;
					await _orderBusiness.Update(Order);
					var paymentLink = await _paymentBusiness.CreatePaymentLink(id, $"{Request.Scheme}://{Request.Host}/OrderPage/CancelPayment?token={user.Token}", $"{Request.Scheme}://{Request.Host}/OrderPage/Success?token={user.Token}", user.UserId);

					if (paymentLink == null || !(paymentLink.Status > 0) || paymentLink.Data == null)
					{
						PopupMessage = paymentLink != null && !string.IsNullOrWhiteSpace(paymentLink.Message) ? paymentLink.Message : "Something went wrong. Payments cannot be done as of this moment.";
						return await OnGetAsync(id);
					}
					return Redirect(((CreatePaymentResult)paymentLink.Data).checkoutUrl);
				case string a when a.Contains("removeProduct_"):
					{
						int productId = int.Parse(action.Split('_')[1]);
						if (productId > 0)
						{
							var deleteResult = await _orderDetailBusiness.DeleteByProductIdAndOrderId(id, productId);
							if (deleteResult == null || !(deleteResult.Status > 0))
							{
								PopupMessage = "Removing the product from this order has failed.";
							}
							else
							{
								PopupMessage = "The product has been removed from this order.";
							}
						}
						return await OnGetAsync(id);
					}
			}
			return RedirectToPage(); // Reload the page after the action
		}

		public async Task<bool> GetOrderInfo(int orderId, int? userId)
		{

			var order1 = await _orderBusiness.GetById(orderId);
			if (order1 == null || !(order1.Status > 0) || order1.Data == null)
			{
				return false;
			}
			Order1 = (Order)order1.Data;
			if (string.IsNullOrEmpty(Order1.Status))
			{
				return false;
			}
			if (userId != null && Order1.CustomerId != (int)userId)
			{
				return false;
			}
			if (userId != null && Order1.Status == "Pending")
			{
				var result = await _orderBusiness.GetPendingOrder(orderId, userId);
				if (result == null || !(result.Status > 0) || result.Data == null)
				{
					PopupMessage = result == null || string.IsNullOrWhiteSpace(result.Message) ? "The order's retrieval process has encountered an unexpected error." : result.Message;
					return false;
				}
				Order1 = (Order)result.Data;
			}
			if (Order1.CouponId != null)
			{
				var result = await _couponBusiness.GetById((int)Order1.CouponId);
				if (result == null || !(result.Status > 0) || result.Data == null)
				{
					Order1.CouponId = null;
					var orderResult = await _orderBusiness.Update(Order1);
					if (orderResult == null || !(orderResult.Status > 0))
					{
						return false;
					}
				}
				else
				{
					var coupon = (Coupon)result.Data;
					CouponCode = coupon.Code;
					CouponMessage = $"Coupon applied! You saved {coupon.DiscountPercentage}%";

				}
			}
			Details = await _orderDetailBusiness.GetDetailsByOrderId(orderId);
			if (Details == null || !(Details.Count > 0))
			{
				return false;
			}
			int i = 0;
			while (i < Details.Count)
			{
				if (Details[i].Product == null || !Details[i].Product.IsAvailable)
				{
					var orderDetailId = Details[i].OrderDetailId;
					Details.RemoveAt(i);
					await _orderDetailBusiness.DeleteById(orderDetailId);
				}
				else
				{
					Products.Add(Details[i].Product);
					Total += Details[i].Price;
					i++;
				}
			}
			if (Total > Order1.TotalAmount)
			{
				Discount = Total - Order1.TotalAmount;
			}
			return true;
		}

		public async Task<IActionResult> OnPostReturnToCartAsync(int orderId)
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

			var order1 = await _orderBusiness.GetPendingOrder(orderId, User1.UserId);
			//if (order1 == null || !(order1.Status > 0) || order1.Data == null)
			//{
			//	order1 = await _orderBusiness.GetSinglePendingOrder(user.UserId);
			//}
			if (order1 == null || !(order1.Status > 0) || order1.Data == null)
			{
				return Page();
			}
			Order1 = (Order)order1.Data;
			if (Order1.OrderDetails == null || Order1.OrderDetails.Count <= 0)
			{
				Order1.OrderDetails = await _orderDetailBusiness.GetDetailsByOrderId(orderId);
				if (Order1.OrderDetails == null || !(Order1.OrderDetails.Count > 0))
				{
					return await OnGetAsync(Order1.OrderId);
				}
			}
			while (Order1.OrderDetails.Count > 0)
			{
				//var newCartProduct = new ShoppingCart
				//{
				//	AddedDate = DateTime.Now,
				//	ProductId = orderDetail.ProductId,
				//	UserId = user.UserId
				//};
				//var result = await _shoppingCartBusiness.Save(newCartProduct);
				//if (result == null || !(result.Status > 0))
				//{
				//	return await OnGetAsync(Order1.OrderId);
				//}
				var orderDetail = Order1.OrderDetails.First();
				Order1.OrderDetails.Remove(orderDetail);
				var result = await _orderDetailBusiness.DeleteById(orderDetail.OrderId);
				if (result == null || !(result.Status > 0))
				{
					return await OnGetAsync(Order1.OrderId);
				}
			}
			var orderResult = await _orderBusiness.DeleteById(Order1.OrderId);
			if (orderResult == null || !(orderResult.Status > 0))
			{
				return await OnGetAsync(Order1.OrderId);
			}
			return new JsonResult(new { redirectUrl = Url.Page("/UserPage/Cart") });
		}
	}
}