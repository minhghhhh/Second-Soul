using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages
{
    public class AddToCartModel : PageModel
    {
        private readonly IProductBusiness _productBusiness;
        private readonly IShoppingCartBusiness _shoppingCartBusiness;
        private readonly IUserBusiness _userBusiness;
        public AddToCartModel(IProductBusiness productBusiness, IUserBusiness userBusiness, IShoppingCartBusiness shoppingCartBusiness)
        {
            _userBusiness = userBusiness;
            _productBusiness = productBusiness;
            _shoppingCartBusiness = shoppingCartBusiness;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var result = await _productBusiness.GetById(id);
            if (result == null || result.Status <= 0 || result.Data == null)
            {
                return RedirectToPage("/Index");
            }
            var product = (Product)result.Data;
            if (!product.IsAvailable || product.SellerId == user.UserId)
            {
                return RedirectToPage("/Error");
            }
            var newCartProduct = new ShoppingCart
            {
                AddedDate = DateTime.Now,
                ProductId = id,
                UserId = user.UserId
            };
            await _shoppingCartBusiness.Save(newCartProduct);
            return RedirectToPage("/Userpage/Cart");
        }

    }
}
