using Data.Base;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class ProductRepo : GenericRepo<Product>
    {
        private readonly SecondSoulShopContext _dbcontext;
        private readonly CategoryRepo _categoryRepo;
        private readonly UserRepo _userRepo;
        public ProductRepo(SecondSoulShopContext context, CategoryRepo categoryRepo, UserRepo userRepo) : base(context)
        {
            _categoryRepo = categoryRepo;
            _dbcontext = context;
            _userRepo = userRepo;
        }
        public List<Product> SortPriceHighToLow(List<Product> products)
        {
            return products
                .Where(p => p.IsAvailable)  
                .OrderByDescending(p => p.Price)  
                .ToList();
        }
        public List<Product> SortPriceLowToHigh(List<Product> products)
        {
            return products
                .Where(p => p.IsAvailable)
                .OrderBy(p => p.Price)
                .ToList();
        }
        public List<Product> SortNewestProduct(List<Product> products)
        {
            return products
                .Where(p => p.IsAvailable)
                .OrderByDescending(p => p.AddedDate)
                .ToList();
        }
        public List<Product> SortOldestProduct(List<Product> products)
        {
            return products
                .Where(p => p.IsAvailable)
                .OrderBy(p => p.AddedDate)
                .ToList();
        }
        public async Task<List<Product>> GetAllProduct()
		{
			return await _dbcontext.Products.Include(a=> a.Category).Include(a=>a.ProductImages).ToListAsync();
		} 
		public async Task<List<Product>> GetAllSellProduct()
		{
			return await _dbcontext.Products.Where(a => a.IsSale == true && a.IsAvailable==true).ToListAsync();
		}
        public async Task<List<Product>> GetProductPriceHighToLow()
        {
            return await _dbcontext.Products.AsNoTracking().OrderByDescending(p => p.Price).Where(p => p.IsAvailable == true).ToListAsync();
        }
        public async Task<List<Product>> GetProductPriceLowToHigh()
        {
            return await _dbcontext.Products.AsNoTracking().OrderBy(p => p.Price).Where(p => p.IsAvailable == true).ToListAsync();
        }

        public async Task<List<Product>> GetProductsNewest()
        {
            return await _dbcontext.Products.AsNoTracking().OrderByDescending(p=>p.AddedDate).Where(p=> p.IsAvailable == true).ToListAsync();
        }
        public async Task<List<Product>> GetProductOldest()
        {
            return await _dbcontext.Products.AsNoTracking().OrderBy(p => p.AddedDate).Where(p => p.IsAvailable == true).ToListAsync();
        }
        public async Task<List<Product>> GetProductsBySeller(int id)
        {
            return await _dbcontext.Products.Where(p=>p.SellerId== id && p.IsAvailable == true).ToListAsync();   
        }
        public IQueryable<List<Product>> GetProductsAsQueryable()
        {
            return (IQueryable<List<Product>>)_dbcontext.Products.AsQueryable();
        }

		public async Task<List<Product>> SearchProduct(string? query, int? minPrice, int? maxPrice, List<int>? categoryIDs, string? condition, bool? isAvailable, int? sellerID, int pageIndex = 1, int pageSize = 10)
		{
			// Validate parameters
			await ValidateSearchParametersAsync(minPrice, maxPrice, categoryIDs, isAvailable, sellerID);

			var productQuery = _dbcontext.Products.AsNoTracking().Include(p => p.Category).Include(a=>a.ProductImages).AsQueryable();

			// Apply search filter
			if (!string.IsNullOrEmpty(query))
			{
				productQuery = productQuery.Where(p => p.Name.Contains(query));
			}

			// Apply price filter
			if (minPrice.HasValue)
			{
				productQuery = productQuery.Where(p => p.Price >= minPrice.Value);
			}

			if (maxPrice.HasValue)
			{
				productQuery = productQuery.Where(p => p.Price <= maxPrice.Value);
			}

			// Apply category filter for multiple category IDs
			if (categoryIDs != null && categoryIDs.Any())
			{
				productQuery = productQuery.Where(p => categoryIDs.Contains(p.CategoryId));
			}

			// Apply condition filter
			if (!string.IsNullOrEmpty(condition))
			{
				productQuery = productQuery.Where(p => p.Condition == condition);
			}

			// Apply availability filter
			if (isAvailable.HasValue)
			{
				productQuery = productQuery.Where(p => p.IsAvailable == isAvailable.Value);
			}

			// Apply seller filter
			if (sellerID.HasValue)
			{
				productQuery = productQuery.Where(p => p.SellerId == sellerID.Value);
			}

			// Execute the query and return the result
			return await productQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
		}

		private async Task ValidateSearchParametersAsync(decimal? minPrice, decimal? maxPrice, List<int>? categoryIDs, bool? isAvailable, int? sellerID)
		{
			// Ensure minPrice is not negative
			if (minPrice.HasValue && minPrice.Value < 0)
			{
				throw new ArgumentException("Minimum price cannot be negative.");
			}

			// Ensure maxPrice is not less than minPrice
			if (minPrice.HasValue && maxPrice.HasValue && maxPrice.Value < minPrice.Value)
			{
				throw new ArgumentException("Maximum price cannot be less than minimum price.");
			}

			// Validate CategoryID asynchronously
			if (categoryIDs != null)
			{
				foreach (var categoryID in categoryIDs)
				{
					if (!(categoryID > 0) || !(await IsValidCategoryAsync(categoryID)))
					{
						throw new ArgumentException("Invalid category ID.");
					}
				}
			}

			// Ensure isAvailable is true (if required in the business logic)
			if (isAvailable.HasValue && isAvailable.Value == false)
			{
				throw new ArgumentException("The product must be available.");
			}

			// Ensure sellerID is not null
			if (sellerID.HasValue)
			{
				if (!(sellerID > 0) || !(await IsValidSellerAsync((int)sellerID)))
				{
					throw new ArgumentException("The seller must be available.");

				}
			}
		}
		public async Task<Product?> GetProductDetails(int id)
		{
			return await context.Products.Include(a=>a.Category).Include(a=>a.Seller).Where(a=>a.ProductId == id && a.IsAvailable== true).FirstOrDefaultAsync();
		}
		private async Task<bool> IsValidCategoryAsync(int categoryID)
		{
			return await _categoryRepo.GetSingleOrDefaultWithNoTracking(c => c.CategoryId == categoryID) != null;
		}

		private async Task<bool> IsValidSellerAsync(int sellerId)
		{
			return await _userRepo.GetSingleOrDefaultWithNoTracking(c => c.UserId == sellerId) != null;
		}
	}

}

