using BusinessObject;
using BusssinessObject;
using Data.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using static Data.Enum.Enums;

namespace Second_Soul.Pages.UserPage.Profile
{
    public class ProductModel : PageModel
    {
        private readonly IProductBusiness _productBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IProductImageBusiness _productImageBusiness;
        private readonly ICategoryBusiness _categoryBusiness;


        public ProductModel(IProductBusiness productBusiness, IUserBusiness userBusiness, IProductImageBusiness productImageBusiness, ICategoryBusiness categoryBusiness)
        {
            _categoryBusiness = categoryBusiness;
            _userBusiness = userBusiness;
            _productBusiness = productBusiness;
            _productImageBusiness = productImageBusiness;
        }
        [BindProperty]
        public string ErrorMessage { get; set; }
        [BindProperty]
        public string SuccessMessage { get; set; }
        public Product product { get; set; }
        public List<Product> products { get; set; }
        public List<ProductImage> Images { get; set; }
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
            [Required]
            public string Size { get; set; } = string.Empty;
            public bool IsAvailable { get; set; } = true;
        }
        public List<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
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
            products = await _productBusiness.GetProductsBySeller(user.UserId);
            if (products.Count == 0)
            {
                ErrorMessage = "You don't have any product";
            }

            var categoryResult = await _categoryBusiness.GetAll();
            if (categoryResult == null || !(categoryResult.Status > 0) || categoryResult.Data == null)
            {
                return RedirectToPage("/Error");
            }

            TempImageUrls = TempData.Get<List<string>>("TempImageUrls") ?? new List<string>();

            // Group categories by ParentCategoryID
            Sizes = Enum.GetNames(typeof(Size)).Select(o => new SelectListItem
            {
                Text = o.Replace("two", "2"),
                Value = o,
                Selected = Product.Size == o.ToString()
            }).ToList();
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
        public async Task<IActionResult> OnPostSortDate(DateTime fromDate, DateTime toDate)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user != null)
            {
                if (fromDate != DateTime.MinValue && toDate != DateTime.MinValue)
                {
                    products = await _productBusiness.GetFilterdAccountProduct(fromDate, toDate, user.UserId);
                    if (products.Count > 0)
                    {
                        return Page();
                    }
                    else
                    {
                        ErrorMessage = $"No Order's from {fromDate} to {toDate}";
                        return Page();
                    }
                }
                else
                {
                    products = await _productBusiness.GetProductsBySeller(user.UserId);
                }
                return Page();
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> OnPostAddProduct()
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
                        Size = Product.Size,
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
