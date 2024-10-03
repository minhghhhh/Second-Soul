using BusinessObject;
using BusssinessObject;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace Second_Soul.Pages
{
    public class SearchModel : PageModel
    {
        private readonly ProductBusiness _productBusiness;
        private readonly CategoryBusiness _categoryBusiness;


        public SearchModel(ProductBusiness productBusiness,CategoryBusiness categoryBusiness)
        {
            _productBusiness = productBusiness;
            _categoryBusiness = categoryBusiness;
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

            Products = (List<Product>)await _productBusiness.SearchProduct(query, minPrice, maxPrice, categoryIDs, condition, isAvailable, sellerID, pageIndex, PageSize);
            Categories = await _categoryBusiness.GetCategoriesAsync();

            // Pagination logic
            TotalPages = (int)Math.Ceiling(Products.Count() / (double)PageSize);
            return Page();
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }

}

