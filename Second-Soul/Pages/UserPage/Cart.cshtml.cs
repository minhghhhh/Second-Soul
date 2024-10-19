using BusssinessObject;
using CloudinaryDotNet;
using Data.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Second_Soul.Pages.UserPage
{
    public class CartModel : PageModel
    {
        private readonly IShoppingCartBusiness _shoppingCartBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IProductBusiness _productBusiness;
        private readonly IOrderBusiness _orderBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;

        public CartModel(IShoppingCartBusiness shoppingCartBusiness, IProductBusiness productBusiness, IUserBusiness userBusiness, IOrderBusiness orderBusiness, IOrderDetailBusiness orderDetailBusiness)
        {
            _productBusiness = productBusiness;
            _shoppingCartBusiness = shoppingCartBusiness;
            _userBusiness = userBusiness;
            _orderBusiness = orderBusiness;
            _orderDetailBusiness = orderDetailBusiness;
        }

        public List<ShoppingCart> ShoppingCarts { get; set; } = [];
        [BindProperty]
        public List<int> SelectedProducts { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            var result = await _shoppingCartBusiness.GetByUserId(user.UserId, null, null); // Modify this for actual pagination in future
            if (result == null || !(result.Status > 0))
            {
                return RedirectToPage("/Error");
            }
            if (result.Status == 4)
            {
                return Page();
            }
            if (result.Data == null)
            {
                return RedirectToPage("/Error");
            }
            ShoppingCarts = (List<ShoppingCart>)result.Data;
            return Page();

        }
        public async Task<IActionResult> OnGetLoadMore(int offset = 0, int limit = 10)
        {
			var user = await _userBusiness.GetFromCookie(Request);
			if (user != null)
			{
				var Totalprice = await _shoppingCartBusiness.PriceCart(user.UserId);
				HttpContext.Session.SetInt32("TotalPrice", Totalprice);
				var results = await _shoppingCartBusiness.GetByUserId(user.UserId, null, null);
				var totalProduct = (List<ShoppingCart>)results.Data;
				HttpContext.Session.SetInt32("TotalProduct", totalProduct.Count());
			}
            else
            {
                return RedirectToPage("/Login");
            }

            var result = await _shoppingCartBusiness.GetByUserId(user.UserId, offset, limit);
            if (result != null && result.Status > 0 && result.Data != null)
            {
                var data = result.Data as List<ShoppingCart>;
                if (data != null)
                {
                    var moreItems = data;
                    return new JsonResult(moreItems);
                }
            }

            return new JsonResult(new List<ShoppingCart>());
        }
        public async Task<IActionResult> OnPostSubmitSelectedItems(string action)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            switch(action)
            {
                case "payment":
					if (SelectedProducts != null && SelectedProducts.Count > 0)
					{
						var phone = user.PhoneNumber ?? string.Empty;
						var address = user.Address ?? string.Empty;
                        int total = 0;
                        foreach(var productId in SelectedProducts)
                        {
                            var result = await _productBusiness.GetById(productId);
                            if (result == null || result.Status <= 0 || result.Data == null) 
                            {
                                return await OnGet();
                            }
							total += ((Product)result.Data).Price;
						}
						int orderId = await _orderBusiness.CreateOrderAsync(user.UserId, SelectedProducts, phone, address, total, null);
						return RedirectToPage("/OrderPage/index", new { id = orderId });
					}
                    break;
                case "delete":
					if (SelectedProducts != null && SelectedProducts.Count > 0)
					{
                        foreach(var product in SelectedProducts)
                        {
							var result = await _shoppingCartBusiness.RemoveFromCart(user.UserId, product);
							if (result == null || result.Status <= 0)
							{

								ModelState.AddModelError(string.Empty, "Removing a product from cart has failed.");
                                return await OnGet();
							}
						}
                        return await OnGet();
					}
					break;
			}

			ModelState.AddModelError(string.Empty, "Please select at least one product.");
            return Page();
        }
    }
}
