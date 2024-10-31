using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace Second_Soul.Pages.ProductPage
{
    public class DetailsModel : PageModel
    {
        private readonly IProductBusiness _productBusiness;
        private readonly IProductImageBusiness _productImageBusiness;
        private readonly IShoppingCartBusiness _shoppingCartBusiness;
        private readonly IOrderBusiness _orderBusiness;
        private readonly IUserBusiness _userBusiness;
        public DetailsModel(IProductBusiness productBusiness , IUserBusiness userBusiness, IProductImageBusiness productImageBusiness, IShoppingCartBusiness shoppingCartBusiness, IOrderBusiness orderBusiness)
        {
            _userBusiness = userBusiness;
            _productBusiness = productBusiness;
            _productImageBusiness = productImageBusiness;
            _shoppingCartBusiness = shoppingCartBusiness;
            _orderBusiness = orderBusiness;
        }

        [BindProperty]
        public bool isSeller { get; set; } = false;

        [BindProperty]
        public Product Product { get; set; }
        public List<ProductImage> Images { get; set; } 
        public async Task<IActionResult> OnGetAsync(int id)
        {
			var user = await _userBusiness.GetFromCookie(Request);
			if (user != null)
			{
				var Totalprice = await _shoppingCartBusiness.PriceCart(user.UserId);
				HttpContext.Session.SetInt32("TotalPrice", Totalprice);
				var result = await _shoppingCartBusiness.GetByUserId(user.UserId, null, null);
				var totalProduct = (List<ShoppingCart>)result.Data;
				HttpContext.Session.SetInt32("TotalProduct", totalProduct.Count());
			}

			var product = await _productBusiness.GetById(id);
            if (product == null || product.Status <= 0 || product.Data == null)
            {
                return RedirectToPage("/Search");
            }
            Product = (Product)product.Data;
            if(user.UserId == Product.SellerId)
            {
                isSeller = true;
            }
            var images = await _productImageBusiness.GetByProductId(id);
            if (images != null && images.Status > 0 && images.Data != null)
            {
                Images = (List<ProductImage>)images.Data;
            }
            if (Product == null)
            {
                return RedirectToPage("/Search");
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string action,int id)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            switch (action)
            {
                case "addToCart":
                    var newCartProduct = new ShoppingCart
                    {
                        AddedDate = DateTime.Now,
                        ProductId = id,
                        UserId = user.UserId
                    };
                    await _shoppingCartBusiness.Save(newCartProduct);
                    return await OnGetAsync(id);
                case "buyNow":
                    List<int> temp = [id];
                    var phone = user.PhoneNumber ?? string.Empty; 
                    var address = user.Address ?? string.Empty;
                    int orderId = await _orderBusiness.CreateOrderAsync(user.UserId, temp, user.FullName, phone, address, 0, null);
                    return RedirectToPage("/OrderPage/Index", new { id = orderId });
            }
            return await OnGetAsync(id);
        }
    }
}
