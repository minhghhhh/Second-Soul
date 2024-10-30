using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace Second_Soul.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProductBusiness _productBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IShoppingCartBusiness _shoppingCartBusiness;
        public IndexModel(IProductBusiness productBusiness, IUserBusiness userBusiness, IShoppingCartBusiness shoppingCartBusiness)
        {
            _productBusiness = productBusiness;
            _userBusiness = userBusiness;
            _shoppingCartBusiness = shoppingCartBusiness;
        }
        public List<Product> NewProducts { get; set; } = new List<Product>();
        public List<Product> SaleProducts { get; set; } = new List<Product>();
        public async Task<IActionResult> OnGet()
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user != null)
            {
                var Totalprice = await _shoppingCartBusiness.PriceCart(user.UserId);
                HttpContext.Session.SetInt32("TotalPrice", Totalprice);
                var result = await _shoppingCartBusiness.GetByUserId(user.UserId, null, null);
                if (result != null && result.Status > 0 && result.Data != null)
                {
                    var totalProduct = (List<ShoppingCart>)result.Data;
                    if (totalProduct != null && totalProduct.Count > 0)
                    {
                        HttpContext.Session.SetInt32("TotalProduct", totalProduct.Count);
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("TotalProduct", 0);
                    }
                }
            }
            NewProducts = await _productBusiness.GetNewestProducts();
            SaleProducts = await _productBusiness.GetAllSellProduct();
            return Page();
        }
    }
}
