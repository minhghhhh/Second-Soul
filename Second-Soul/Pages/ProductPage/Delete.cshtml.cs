using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.ProductPage
{
    public class DeleteModel : PageModel
    {
        private readonly IUserBusiness _userBusiness;
        private readonly IProductBusiness _productBusiness;
        public DeleteModel(IUserBusiness userBusiness, IProductBusiness productBusiness)
        {
            _userBusiness = userBusiness;
            _productBusiness = productBusiness;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            var product = await _productBusiness.GetById(id);
            if (product == null || product.Status <= 0 || product.Data == null)
            {
                return RedirectToPage("/Error");
            }
            var Product = (Product)product.Data;
            if(Product.IsAvailable == false)
            {
                return RedirectToPage("/Error");
            }
            if (user.UserId != Product.SellerId)
            {
                return RedirectToPage("/Error");
            }
            Product.IsAvailable = false;
            await _productBusiness.Update(Product);
            return RedirectToPage("/UserPage/Profile/Product");
        }
    }
}
