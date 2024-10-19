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
		private readonly IUserBusiness _userBusiness;


		public SearchModel(IProductBusiness productBusiness, ICategoryBusiness categoryBusiness, IUserBusiness userBusiness)
		{
			_productBusiness = productBusiness;
			_categoryBusiness = categoryBusiness;
			_userBusiness = userBusiness;
		}
		public string? Query { get; set; }
		public int? MinPrice { get; set; }
		public int? MaxPrice { get; set; }
		public List<SelectListItem> Conditions { get; set; } = new List<SelectListItem>();
		public string? Condition { get; set; }
		public bool IsAvailable { get; set; } = true;
		public List<SelectListItem> Sellers { get; set; } = new List<SelectListItem>();
		public List<Product> Products { get; set; } = new List<Product>();
		public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
		public int PageIndex { get; set; } = 1;
		public int TotalPages { get; set; } = 0;
		public int PageSize { get; set; } = 10; // Default items per page

		// OnGet method to handle the search logic
		public async Task<IActionResult> OnGetAsync(string? query, int? minPrice, int? maxPrice, List<int>? categoryIDs, string? condition, bool? isAvailable, int? sellerID, int pageIndex = 1)
		{
			Query = query;
			MinPrice = minPrice;
			MaxPrice = maxPrice;
			Condition = string.IsNullOrEmpty(condition) ? null : condition;

			Conditions = Enum.GetNames(typeof(Condition)).Select(o => new SelectListItem
			{
				Text = o.Replace('_', ' '),
				Value = o,
				Selected = condition != null ? o == Condition : false
			}).ToList();

			IsAvailable = isAvailable != null && (bool)isAvailable;
			PageIndex = pageIndex;

			var productResult = await _productBusiness.SearchProduct(query, minPrice, maxPrice, categoryIDs, condition, isAvailable, sellerID, pageIndex, PageSize);
			if (productResult.Status > 0 && productResult.Data != null)
			{
				Products = (List<Product>)productResult.Data;
			}

			// Update the method that fetches categories
			var categoryResult = await _categoryBusiness.GetAll();
			if (categoryResult != null && categoryResult.Status > 0 && categoryResult.Data != null)
			{
				var categories = categoryResult.Data as List<Category>;
				if (categories != null && categories.Count > 0)
				{
					var parentCategories = categories.Where(c => c.ParentCategoryId == null).ToList();
					foreach (Category parentCategory in parentCategories)
					{
						var subCategories = categories.Where(c => c.ParentCategoryId == parentCategory.CategoryId);
						var selectGroup = new SelectListGroup
						{
							Name = parentCategory.CategoryName,
							Disabled = false
						};
						foreach (Category subCategory in subCategories)
						{
							Categories.Add(new SelectListItem
							{
								Text = subCategory.CategoryName, // Subcategory text
								Value = subCategory.CategoryId.ToString(),
								Selected = categoryIDs != null && categoryIDs.Contains(subCategory.CategoryId),
								Group = selectGroup
							});
						}
					}
				}
			}
			var sellerResult = await _userBusiness.ReadOnlyActiveSellers();
			if (sellerResult != null && sellerResult.Status > 0 && sellerResult.Data != null)
			{
				var sellers = sellerResult.Data as List<User>;
				if (sellers != null && sellers.Count > 0)
				{
					foreach (User seller in sellers)
					{
						Sellers.Add(new SelectListItem
						{
							Text = seller.Username,
							Value = seller.UserId.ToString(),
							Selected = sellerID != null && seller.UserId == sellerID
						});
					}
				}
			}

			// Pagination logic
			TotalPages = (int)Math.Ceiling(Products.Count() / (double)PageSize);
			return Page();
		}

		public bool HasPreviousPage => PageIndex > 1;
		public bool HasNextPage => PageIndex < TotalPages;
	}

}

