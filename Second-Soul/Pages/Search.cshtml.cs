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
        private readonly IShoppingCartBusiness _shoppingCartBusiness;

        public SearchModel(IProductBusiness productBusiness, ICategoryBusiness categoryBusiness, IUserBusiness userBusiness, IShoppingCartBusiness shoppingCartBusiness)
        {
            _shoppingCartBusiness = shoppingCartBusiness;
            _productBusiness = productBusiness;
            _categoryBusiness = categoryBusiness;
            _userBusiness = userBusiness;
        }
        [BindProperty(SupportsGet = true)]
        public string SortOption { get; set; } = "Newest";
        public List<SelectListItem> SortOptions { get; set; } = new();
        public string? Query { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public List<SelectListItem> Conditions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
        public int TotalResults { get; set; } = 0;
        public string? Condition { get; set; }
        public string? Size { get; set; }
        public string FirstIndex { get; set; } = "??";
        public string LastIndex { get; set; } = "??";
        public bool IsAvailable { get; set; } = true;
        public List<SelectListItem> Sellers { get; set; } = new List<SelectListItem>();
        public List<Product> Products { get; set; } = new List<Product>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; } = 0;
        public int PageSize { get; set; } = 10; // Default items per page

        // OnGet method to handle the search logic
        public async Task<IActionResult> OnGetAsync(string? query, int? minPrice, int? maxPrice, string? size, List<int>? categoryIDs, string? condition, bool? isAvailable, int? sellerID, int pageIndex = 1)
        {
            var user = await _userBusiness.GetFromCookie(Request);
            if (user != null)
            {
                var Totalprice = await _shoppingCartBusiness.PriceCart(user.UserId);
                HttpContext.Session.SetInt32("TotalPrice", Totalprice);
                var result = await _shoppingCartBusiness.GetByUserId(user.UserId, null, null);
                if (result != null && result.Status > 0 && result.Data != null)
                {
                    var totalProduct = (List<ShoppingCart>)result.Data;
                    if (totalProduct != null && totalProduct.Count > 0)
                    {
                        HttpContext.Session.SetInt32("TotalProduct", totalProduct.Count);
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("TotalProduct", 0);
                    }
                }
            }
            Query = query;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            Condition = string.IsNullOrEmpty(condition) ? null : condition;
            Size = string.IsNullOrEmpty(size) ? null : size;


            Conditions = Enum.GetNames(typeof(Condition)).Select(o => new SelectListItem
            {
                Text = o.Replace('_', ' '),
                Value = o,
                Selected = condition != null ? o == Condition : false
            }).ToList();
            Sizes = Enum.GetNames(typeof(Size)).Select(o => new SelectListItem
            {
                Text = o.Replace("two", "2"),
                Value = o,
                Selected = size != null ? o == Size : false
            }).ToList();
            IsAvailable = isAvailable != null && (bool)isAvailable;
            PageIndex = pageIndex;

            var productResult = await _productBusiness.SearchProduct(query, minPrice, maxPrice, categoryIDs, condition, size, isAvailable, sellerID);
            if (productResult.Status > 0 && productResult.Data != null)
            {
                Products = (List<Product>)productResult.Data;
                TotalResults = Products.Count();
                TotalPages = (int)Math.Ceiling(Products.Count() / (double)PageSize);
                Products = Products.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                if (Products.Count > 0)
                {
                    int first = PageSize * PageIndex - PageSize + 1;
					FirstIndex = first.ToString();
                    LastIndex = (Math.Min(Math.Abs(TotalResults - first), 9) + first).ToString();
				}
            }

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
                                Text = subCategory.CategoryName,
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
            SortOptions = new List<SelectListItem>
        {
            new SelectListItem { Value = "PriceAsc", Text = "Lowest to Highest Price" },
            new SelectListItem { Value = "PriceDesc", Text = "Highest to Lowest Price" },
            new SelectListItem { Value = "Newest", Text = "Newest" },
            new SelectListItem { Value = "Oldest", Text = "Oldest" }
        };
            if (string.IsNullOrEmpty(SortOption))
            {
                SortOption = "Newest";
            }
            Products = SortProducts(Products, SortOption);

            return Page();
        }
        private List<Product> SortProducts(List<Product> products, string sortOption)
        {
            switch (sortOption)
            {
                case "PriceAsc":
                    return _productBusiness.SortPriceLowToHigh(products);
                case "PriceDesc":
                    return _productBusiness.SortPriceHighToLow(products);
                case "Newest":
                    return _productBusiness.SortNewestProduct(products);
                case "Oldest":
                    return _productBusiness.SortOldestProduct(products);
            }
            return products;
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }

}
