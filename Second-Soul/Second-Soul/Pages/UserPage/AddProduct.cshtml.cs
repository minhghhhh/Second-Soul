using BusinessObject;
using BusssinessObject;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Second_Soul.Pages.UserPage
{
	public class AddProductModel : PageModel
	{
		private readonly IProductBusiness _productBusiness;
		private readonly ICategoryBusiness _categoryBusiness;
		private readonly IProductImageBusiness _productImageBusiness;
		private readonly IUserBusiness _userBusiness;

		public AddProductModel(IProductBusiness productBusiness, ICategoryBusiness categoryBusiness, IProductImageBusiness productImageBusiness, IUserBusiness userBusiness)
		{
			_userBusiness = userBusiness;
			_productBusiness = productBusiness;
			_categoryBusiness = categoryBusiness;
			_productImageBusiness = productImageBusiness;
		}

		public class ProductInputModel
		{
			[Required]
			public string Name { get; set; } = string.Empty;
			public string? Description { get; set; }
			[Required]
			[Range(1, int.MaxValue, ErrorMessage = "Price must be a positive value")]
			public int Price { get; set; }
			[Required]
			public int CategoryID { get; set; }
			[Required]
			public string Condition { get; set; } = string.Empty;
			public bool IsAvailable { get; set; } = true;
		}

		[BindProperty]
		public ProductInputModel Product { get; set; } = new();

		[BindProperty]
		[Required]
		[Display(Name = "Upload Image")]
		[DataType(DataType.Upload)]
		public IFormFile ProductImage { get; set; }

		public List<string> TempImageUrls { get; set; } = new List<string>();
		public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

		[BindProperty]
		public IFormFile[] Photos { get; set; } = Array.Empty<IFormFile>();
		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userBusiness.GetFromCookie(Request);
			if (user == null)
			{
				return RedirectToPage("/Login");
			}

			var categoryResult = await _categoryBusiness.GetAll();
			if (categoryResult == null || !(categoryResult.Status > 0) || categoryResult.Data == null)
			{
				return RedirectToPage("/Error");
			}

			TempImageUrls = TempData.Get<List<string>>("TempImageUrls") ?? new List<string>();

			// Group categories by ParentCategoryID
			var categories = (List<Category>)categoryResult.Data;
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
							Group = selectGroup
						});
					}
				}
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				var user = await _userBusiness.GetFromCookie(Request);
				if (user == null)
				{
					return RedirectToPage("/Login");
				}

				if (Photos == null || !(Photos.Length > 0))
				{
					return await OnGetAsync();
				}

				var product = new Product
				{
					Name = Product.Name,
					SellerId = user.UserId,
					Description = Product.Description ?? string.Empty,
					Price = Product.Price,
					CategoryId = Product.CategoryID,
					Condition = Product.Condition,
					AddedDate = DateTime.Now,
					IsAvailable = true
				};


				// Save the product (assuming your _productBusiness has the Save method)
				await _productBusiness.Save(product);

				// Upload Images (Example - Adjust based on your _productImageBusiness)
				foreach (var file in Photos)
				{
					if (file.Length > 0)
					{
						var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
						var fileExtension = Path.GetExtension(file.FileName).ToLower();
						if (!allowedExtensions.Contains(fileExtension))
						{
							return Page();
						}
						var uploadResult = await _productImageBusiness.UploadImageAsync(file);
						var productImage = new ProductImage
						{
							ProductId = product.ProductId,
							ImageUrl = uploadResult
						};
						await _productImageBusiness.Save(productImage);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return StatusCode(500, "Error saving product");
			}

			return RedirectToPage("/Index");
		}
	}

	public static class TempDataExtensions
	{
		public static void Set<T>(this ITempDataDictionary tempData, string key, T value) where T : class
		{
			tempData[key] = JsonConvert.SerializeObject(value);
		}

		public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
		{
			object o;
			tempData.TryGetValue(key, out o);
			return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
		}
	}
}