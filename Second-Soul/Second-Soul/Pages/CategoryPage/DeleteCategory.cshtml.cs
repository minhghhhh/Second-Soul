using BusinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.CategoryPage
{
    public class DeleteCategoryModel : PageModel
    {
        private readonly ICategoryBusiness _business;

        /*public DeleteModel(InternData.Models.NET1702_PRN221_InternContext context)
        {
            _context = context;
        }*/

        public DeleteCategoryModel(ICategoryBusiness categoryBusiness)
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _business.DeleteById(id);
            }
            catch (Exception ex)
            {
            }

            return RedirectToPage("./IndexCategory");

        }
    }
}
