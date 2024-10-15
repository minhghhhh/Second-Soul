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
        public List<Category> Categories { get; set; } = new();

        public IFormFile[] Photos { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var categories = await _categoryBusiness.GetAll();
            if (categories == null || !(categories.Status > 0) || categories.Data == null)
            {
                return RedirectToPage("/Error");
            }
            Categories = (List<Category>)categories.Data;
            TempImageUrls = TempData.Get<List<string>>("TempImageUrls") ?? new List<string>();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var formData = Request.Form;
            var product = new Product
            {
                Name = Product.Name,
                Description = Product.Description ?? string.Empty,
                Price = Product.Price,
                CategoryId = Product.CategoryID,
                Condition = Product.Condition,
                AddedDate = DateTime.Now,
                IsAvailable = true // Set other necessary fields
            };

            await _productBusiness.Save(product);

            // Handling file uploads
            var files = Photos;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Upload each file (e.g., to Cloudinary)
                    var uploadResult = await _productImageBusiness.UploadImageAsync(file);
                    var productImage = new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = uploadResult // The returned Cloudinary URL
                    };
                    await _productImageBusiness.Save(productImage);
                }
            }

            return new JsonResult(new { success = true });
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