using BusssinessObject;
using CloudinaryDotNet;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.UserPage
{
    public class CartModel : PageModel
    {
        private readonly IShoppingCartBusiness _shoppingCartBusiness;
        private readonly IUserBusiness _userBusiness;

        public CartModel(IShoppingCartBusiness shoppingCartBusiness, IUserBusiness userBusiness)
        {
            _shoppingCartBusiness = shoppingCartBusiness;
            _userBusiness = userBusiness;
        }

        public List<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();

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
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var result = await _shoppingCartBusiness.GetByUserId(user.UserId, offset, limit); // Modify for actual pagination
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
        public async Task<IActionResult> OnPostDeleteItem(int productId)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var result = await _shoppingCartBusiness.RemoveFromCart(user.UserId, productId);
            if (result == null || result.Status <= 0)
            {
                return RedirectToPage("/Error");
            }

            return await OnGet();
        }

    }
}
