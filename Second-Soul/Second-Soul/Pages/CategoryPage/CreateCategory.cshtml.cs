using BusinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.CategoryPage
{
    public class CreateCategoryModel : PageModel
    {
        private readonly ICategoryBusiness _business;

        public CreateCategoryModel(ICategoryBusiness business)
        {
           _business = business;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Category Cate { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (await _business.IdExists(Cate.CategoryId))
            {
                ModelState.AddModelError("Cate.CategoryId", "Id already exists, please create a new one.");
                return Page();
            }

            await _business.Save(Cate);

            return RedirectToPage("./IndexCategory");
        }
    }
}
