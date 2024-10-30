using BusinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.CategoryPage
{
    public class EditCategoryModel : PageModel
    {
        private readonly ICategoryBusiness _business;
        public EditCategoryModel(ICategoryBusiness categoryBusiness)
        {
            _business = categoryBusiness;
        }
        [BindProperty]
        public Category Cate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cate = await _business.GetById(id);

            if (cate == null)
            {
                return NotFound();
            }
            else
            {
                Cate = cate.Data as Category;

                // Trim whitespace from the values
                if (Cate != null)
                {
                    Cate.CategoryName = Cate.CategoryName?.Trim();
                }
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Trim whitespace from the values before saving
            if (Cate != null)
            {
                Cate.CategoryName = Cate.CategoryName?.Trim();
            }

            try
            {
                await _business.Update(Cate);
            }
            catch (Exception ex)
            {
            }

            return RedirectToPage("./IndexCategory");
        }
    }
}
