using BusssinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Service.ProductService;
using Service.UserService;

namespace Second_Soul.Pages
{
    public class SearchModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IProductService _producService;
        public SearchModel(IUserService userService, IProductService producService)
        {
            _producService = producService;
            _userService = userService;
        }
        public string? Query { get; set; }
        public List<string> SearchResults { get; set; } = new List<string>();

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? CategoryID { get; set; }
        public string? Condition { get; set; }
        public bool? IsAvailable { get; set; }
        public int? SellerID { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; } = 0;
        public int PageSize { get; set; } = 10; // Default items per page

        // OnGet method to handle the search logic
        public async Task<IActionResult> OnGetAsync(string? query, decimal? minPrice, decimal? maxPrice, int? categoryID, string? condition, bool? isAvailable, int? sellerID, int pageIndex = 1)
        {

            Query = query;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            CategoryID = categoryID;
            Condition = condition;
            IsAvailable = isAvailable;
            SellerID = sellerID;
            PageIndex = pageIndex;

            Products = await _producService.SearchProduct(query, minPrice, maxPrice, categoryID, condition, isAvailable, sellerID, pageIndex, PageSize);

            // Pagination logic
            TotalPages = (int)Math.Ceiling( Products.Count() / (double)PageSize);
            return Page();

        }

        private List<string> PerformSearch(string query)
        {
            // Mock data - replace with actual database or data source search
            var items = new List<string>
        {
            "Shirt", "Pants", "Shoes", "Second-Hand Jacket",
            "Bags", "Jeans", "Skirts", "Dresses", "Hats", "Coats",
            "Scarves", "Gloves", "T-Shirts", "Belts", "Socks","haha","huhu",  "Shirt", "Pants", "Shoes", "Second-Hand Jacket",
            "Bags", "Jeans", "Skirts", "Dresses", "Hats", "Coats",
            "Scarves", "Gloves", "T-Shirts", "Belts", "Socks"
        };
            if (string.IsNullOrEmpty(query))
            {
                return items.ToList();

            }
            return items.Where(item => item.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }

}

