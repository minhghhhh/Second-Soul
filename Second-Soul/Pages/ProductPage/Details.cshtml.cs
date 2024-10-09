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
        private readonly IUserBusiness _userBusiness;
        public DetailsModel(IProductBusiness productBusiness,IUserBusiness userBusiness, IProductImageBusiness productImageBusiness, IShoppingCartBusiness shoppingCartBusiness)
        {
            _userBusiness = userBusiness;
            _productBusiness = productBusiness;
            _productImageBusiness = productImageBusiness;
            _shoppingCartBusiness = shoppingCartBusiness;
        }

        [BindProperty]
        public Product Product { get; set; }
        public List<ProductImage> Images { get; set; } 
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productBusiness.GetById(id);
            if (product == null || product.Status <= 0 || product.Data == null)
            {
                return RedirectToPage("/Search");
            }
            Product = (Product)product.Data;
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
                    return RedirectToPage("/Order");
            }
           return await OnGetAsync(id);
        }
    }
}
