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
        private readonly IOrderBusiness _orderBusiness;
        private readonly IOrderDetailBusiness _orderDetailBusiness;

        public CartModel(IShoppingCartBusiness shoppingCartBusiness, IUserBusiness userBusiness, IOrderBusiness orderBusiness, IOrderDetailBusiness orderDetailBusiness)
        {
            _shoppingCartBusiness = shoppingCartBusiness;
            _userBusiness = userBusiness;
            _orderBusiness = orderBusiness;
            _orderDetailBusiness = orderDetailBusiness;
        }

        public List<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
        [BindProperty]
        public List<int> SelectedProducts { get; set; } 
        public async Task<IActionResult> OnGet()
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            var result = await _shoppingCartBusiness.GetByUserId(user.UserId, null, null); 
            if (result == null || !(result.Status > 0) || result.Data == null)
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
        public async Task<IActionResult> OnPostProceedToPaymentAsync()
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            if (SelectedProducts != null && SelectedProducts.Count > 0)
            {
                
                return RedirectToPage("/OrderPage", new { selectedProducts = SelectedProducts });
            }

            ModelState.AddModelError("", "Please select at least one product to proceed.");
            return Page();
        }


    }
}
