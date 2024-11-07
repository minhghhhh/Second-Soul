using BusinessObject;
using BusssinessObject;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Second_Soul.Pages.UserPage.Profile;
using System.ComponentModel.DataAnnotations;

namespace Second_Soul.Pages.ProductPage
{
	public class UpdateModel : PageModel
	{
		private readonly IProductBusiness _productBusiness;
		private readonly ICategoryBusiness _categoryBusiness;
		private readonly IProductImageBusiness _productImageBusiness;
		private readonly IUserBusiness _userBusiness;

		public UpdateModel(IProductBusiness productBusiness, ICategoryBusiness categoryBusiness, IProductImageBusiness productImageBusiness, IUserBusiness userBusiness)
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
			[Required]
			public string Description { get; set; } = string.Empty;
			[Required]
			[Range(1, int.MaxValue, ErrorMessage = "Price must be a positive value")]
			public int Price { get; set; }
			[Required]
			public int CategoryID { get; set; }
			[Required]
			public string Condition { get; set; } = string.Empty;
			[Required]
			public string Size { get; set; } = string.Empty;
			[Required]
			public string MainImage { get; set; } = string.Empty;
			public bool IsSale { get; set; } = false;
			public int SalePrice { get; set; } = 0;

		}
		[BindProperty]
		public ProductInputModel Product { get; set; } = new();

		//[BindProperty]
		//[Required]
		//[Display(Name = "Upload Image")]
		//[DataType(DataType.Upload)]
		//public IFormFile ProductImage { get; set; }

		public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

		[BindProperty]
		public IFormFile[] Photos { get; set; } = Array.Empty<IFormFile>();

		public IList<ProductImage> Images { get; set; } = new List<ProductImage>();

		public int Id { get; set; } = 0;
		public async Task<IActionResult> OnGetAsync(int id)
		{
			var user = await _userBusiness.GetFromCookie(Request);
			if (user == null)
			{
				return RedirectToPage("/Login");
			}

			if (!(id > 0))
			{
				return RedirectToPage("/Search");
			}

			var result = await _productBusiness.GetByIdNoAvailable(id);

			if (result == null || !(result.Status > 0) || result.Data == null)
			{
				return RedirectToPage("/Error");
			}
			var product = (Product)result.Data;
			if (user.Role != "Admin" && user.UserId != product.SellerId)
			{
				return RedirectToPage("/Error");
			}
			Product.Name = product.Name;
			Product.Description = product.Description;
			Product.Price = product.Price;
			Product.SalePrice = product.SalePrice ?? 0;
			Product.Size = product.Size;
			Product.CategoryID = product.CategoryId;
			Product.Condition = product.Condition;
			Product.IsSale = product.IsSale;
			Product.MainImage = product.MainImage;
			var imageResult = await _productImageBusiness.GetByProductId(product.ProductId);
			if (!(imageResult.Status > 0) || imageResult.Data == null)
			{
				return RedirectToPage("/Error");
			}
			Images = (List<ProductImage>)imageResult.Data;

			var categoryResult = await _categoryBusiness.GetAll();
			if (!(categoryResult.Status > 0) || categoryResult.Data == null)
			{
				return RedirectToPage("/Error");
			}

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
			Id = id;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(int id)
		{
			try
			{
				var user = await _userBusiness.GetFromCookie(Request);
				if (user == null)
				{
					return RedirectToPage("/Login");
				}

				if (!(id > 0))
				{
					return RedirectToPage("/Error");
				}



				var result = await _productBusiness.GetByIdNoAvailable(id);
				if (!(result.Status > 0) || result.Data == null)
				{
					return RedirectToPage("/Error");
				}

				var product = (Product)result.Data;
				product.Name = Product.Name;
				product.Description = Product.Description;
				product.Price = Product.Price;
				product.SalePrice = (Product.SalePrice > 0) ? Product.SalePrice : null;
				product.IsSale = Product.IsSale && product.SalePrice != null;
				product.Size = Product.Size;
				product.Condition = Product.Condition;
				product.MainImage = Product.MainImage;
				product.CategoryId = Product.CategoryID;


				var deleteImageIds = Request.Form["DeleteImages"].ToList();
				int i = 0;
				while (i < deleteImageIds.Count)
				{
					if (int.TryParse(deleteImageIds[i], out int imageId))
					{
						result = await _productImageBusiness.GetByProductId(product.ProductId);
						if (result.Status > 0 && result.Data != null)
						{
							Images = (List<ProductImage>)result.Data;
							if (Images.Count > deleteImageIds.Count)
							{
								var image = Images.Where(i => i.Id == imageId).FirstOrDefault();
								if (image == null || image.ImageUrl == product.MainImage)
								{
									image = Images.Where(i => i.Id != imageId).FirstOrDefault();
									product.MainImage = image != null && !string.IsNullOrWhiteSpace(image.ImageUrl) ? image.ImageUrl : string.Empty;
								}
								deleteImageIds.RemoveAt(i);
								await _productImageBusiness.DeleteById(imageId);
							}
							else
							{
								i++;
							}
						}
					}

				}

				result = await _productBusiness.Update(product);
				if (!(result.Status > 0))
				{
					return RedirectToPage("/Error");
				}

				if (Photos != null && Photos.Length > 0)
				{

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
			}

			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return StatusCode(500, "Error saving product");
			}

			return Redirect("/ProductPage/Details/" + id);
		}
	}

}
