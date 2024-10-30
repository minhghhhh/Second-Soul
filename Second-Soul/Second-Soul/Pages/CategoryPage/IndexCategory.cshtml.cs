using BusinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Second_Soul.Pages.CategoryPage
{
    public class IndexCategoryModel : PageModel
    {
        private readonly ICategoryBusiness _business;
        public IndexCategoryModel(ICategoryBusiness business)
        {
            _business = business;
        }


        [BindProperty(SupportsGet = true)]
        public int? SearchId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchBySchoolName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }

        public bool IsIdFound { get; set; } = false;
        public bool IsDataFound { get; set; } = true;
        public IList<Category> Cate { get; set; } = default!;
        public async Task OnGetAsync()
        {

            var result = await _business.AdvancedSearch(SearchId, SearchName, SearchId);

            if (result != null && result.Status > 0 && result.Data != null)
            {
                var allCvs = result.Data as List<Category>;
                TotalPages = (int)Math.Ceiling(allCvs.Count / (double)PageSize);
                Cate = allCvs.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                IsDataFound = Cate.Any();
            }
            else
            {
                Cate = new List<Category>();
                IsDataFound = false;
            }
        }
    }
}

