using BusssinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Service.UserService;

namespace Second_Soul.Pages
{
    public class SearchModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IProducService _producService;
        public string Query { get; set; }
        public List<string> SearchResults { get; set; } = new List<string>();

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? CategoryID { get; set; }
        public string Condition { get; set; }
        public bool? IsAvailable { get; set; }
        public int? SellerID { get; set; }


        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10; // Default items per page

        // OnGet method to handle the search logic
        public async Task<IActionResult> OnGetAsync(string query, decimal? minPrice, decimal? maxPrice, int? categoryID, string condition, bool? isAvailable, int? sellerID, int pageIndex = 1)
        {
            Query = query;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            CategoryID = categoryID;
            Condition = condition;
            IsAvailable = isAvailable;
            SellerID = sellerID;
            PageIndex = pageIndex;

            // Query to get products from the database
            var productQuery = _context.Products.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(Query))
            {
                productQuery = productQuery.Where(p => p.Name.Contains(Query));
            }

            // Apply price filter
            if (MinPrice.HasValue)
            {
                productQuery = productQuery.Where(p => p.Price >= MinPrice);
            }
            if (MaxPrice.HasValue)
            {
                productQuery = productQuery.Where(p => p.Price <= MaxPrice);
            }

            // Apply category filter
            if (CategoryID.HasValue)
            {
                productQuery = productQuery.Where(p => p.CategoryID == CategoryID);
            }

            // Apply condition filter
            if (!string.IsNullOrEmpty(Condition))
            {
                productQuery = productQuery.Where(p => p.Condition == Condition);
            }

            // Apply availability filter
            if (IsAvailable.HasValue)
            {
                productQuery = productQuery.Where(p => p.IsAvailable == IsAvailable);
            }

            // Apply seller filter
            if (SellerID.HasValue)
            {
                productQuery = productQuery.Where(p => p.SellerID == SellerID);
            }

            // Pagination logic
            TotalPages = (int)Math.Ceiling(await productQuery.CountAsync() / (double)PageSize);
            Products = await productQuery.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();

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

