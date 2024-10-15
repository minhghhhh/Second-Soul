using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProductBusiness _productBusiness;
        public IndexModel(IProductBusiness productBusiness)
        {
        _productBusiness = productBusiness;
        }
        public List<Product> NewestProducts {  get; set; }
        public async Task<IActionResult> OnGet()
        {
            NewestProducts = await _productBusiness.GetNewestProducts();
            return Page();
        }
    }
}
