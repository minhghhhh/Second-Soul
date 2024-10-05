using BusinessObject;
using BusssinessObject;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Data.Enum.Enums;
namespace Second_Soul.Pages
{
	public class SearchModel : PageModel
	{
		private readonly IProductBusiness _productBusiness;
		private readonly ICategoryBusiness _categoryBusiness;


		public SearchModel(IProductBusiness productBusiness, ICategoryBusiness categoryBusiness)
		{
			_productBusiness = productBusiness;
			_categoryBusiness = categoryBusiness;
		}
		public string? Query { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
		public List<SelectListItem> Conditions { get; set; } = new List<SelectListItem>();
		public string? Condition { get; set; }
		public bool? IsAvailable { get; set; }
		public int? SellerID { get; set; }
		public List<Product> Products { get; set; } = new List<Product>();
		public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
		public int PageIndex { get; set; } = 1;
		public int TotalPages { get; set; } = 0;
		public int PageSize { get; set; } = 10; // Default items per page

		// OnGet method to handle the search logic
		public async Task<IActionResult> OnGetAsync(string? query, decimal? minPrice, decimal? maxPrice, List<int>? categoryIDs, string? condition, bool? isAvailable, int? sellerID, int pageIndex = 1)
		{
			Query = query;
			MinPrice = minPrice;
			MaxPrice = maxPrice;
			Condition = string.IsNullOrEmpty(condition) ? null : condition;

			Conditions = Enum.GetNames(typeof(Condition)).Select(o => new SelectListItem
			{
				Text = o,
				Value = o,
				Selected = condition != null ? o == Condition : false
			}).ToList();
			IsAvailable = isAvailable;
			SellerID = sellerID;
			PageIndex = pageIndex;

			var productResult = await _productBusiness.SearchProduct(query, minPrice, maxPrice, categoryIDs, condition, isAvailable, sellerID, pageIndex, PageSize);
			if (productResult.Status > 0 && productResult.Data != null) 
			{
				Products = (List<Product>) productResult.Data;
			} 
				var result = await _categoryBusiness.GetAll();
			if (result.Status > 0 && result.Data != null)
			{
				var categories = result.Data as List<Category>;
				if (categories != null && categories.Count > 0)
					Categories = categories.Select(c => new SelectListItem
					{
						Text = c.CategoryName,
						Value = c.CategoryId.ToString(),
						Selected = categoryIDs == null ? false : categoryIDs.Contains(c.CategoryId)
					}).ToList();

			}

			// Pagination logic
			TotalPages = (int)Math.Ceiling(Products.Count() / (double)PageSize);
			return Page();
		}

		public bool HasPreviousPage => PageIndex > 1;
		public bool HasNextPage => PageIndex < TotalPages;
	}

}

