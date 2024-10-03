using BusssinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Service.CategoryService;
using Service.ProductService;
using Service.UserService;

namespace Second_Soul.Pages
{
    public class SearchModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IProductService _producService;
        private readonly ICategoryService _categoryService;
        public SearchModel(IUserService userService, IProductService producService, ICategoryService categoryService)
        {
            _producService = producService;
            _userService = userService;
            _categoryService = categoryService;
        }
        public string? Query { get; set; }
        public List<string> SearchResults { get; set; } = new List<string>();

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<int> CategoryIDs { get; set; } = new List<int>();
        public string? Condition { get; set; }
        public bool? IsAvailable { get; set; }
        public int? SellerID { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; } = 0;
        public int PageSize { get; set; } = 10; // Default items per page

        // OnGet method to handle the search logic
        public async Task<IActionResult> OnGetAsync(string? query, decimal? minPrice, decimal? maxPrice, List<int>? categoryIDs, string? condition, bool? isAvailable, int? sellerID, int pageIndex = 1)
        {
            Query = query;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            CategoryIDs = categoryIDs; // Update this line
            Condition = condition;
            IsAvailable = isAvailable;
            SellerID = sellerID;
            PageIndex = pageIndex;

            Products = await _producService.SearchProduct(query, minPrice, maxPrice, categoryIDs, condition, isAvailable, sellerID, pageIndex, PageSize);
            Categories = await _categoryService.GetCategoriesAsync();

            // Pagination logic
            TotalPages = (int)Math.Ceiling(Products.Count() / (double)PageSize);
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

