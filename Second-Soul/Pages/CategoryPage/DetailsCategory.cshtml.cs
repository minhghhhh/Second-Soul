using BusinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.CategoryPage
{
    public class DetailsCategoryModel : PageModel
    {
        private readonly ICategoryBusiness _business;
        public DetailsCategoryModel(ICategoryBusiness categoryBusiness)
        {
            _business = categoryBusiness;
        }

        public Category Cate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ct = await _business.GetById(id);
            if (ct == null)
            {
                return NotFound();
            }
            else
            {
                Cate = ct.Data as Category;
            }
            return Page();
        }
    }
}
