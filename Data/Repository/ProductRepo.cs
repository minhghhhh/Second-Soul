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
        public ProductRepo(SecondSoulShopContext context,CategoryRepo categoryRepo) : base(context)
        {
            _categoryRepo = categoryRepo;
            _dbcontext = context;
        }
        public IQueryable<List<Product>> GetProductsAsQueryable()
        {
            return (IQueryable<List<Product>>)_dbcontext.Products.AsQueryable();
        }
        public async Task<List<Product>> SearchProduct(string query, decimal? minPrice, decimal? maxPrice, List<int>? categoryIDs, string condition, bool? isAvailable, long? sellerID, int pageIndex = 1, int pageSize = 10)
        {
            // Validate parameters
            await ValidateSearchParametersAsync(minPrice, maxPrice, categoryIDs, isAvailable, sellerID);

            var productQuery = _dbcontext.Products.Include(p => p.Category).AsQueryable();

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

        private async Task ValidateSearchParametersAsync(decimal? minPrice, decimal? maxPrice, List<int>? categoryIDs, bool? isAvailable, long? sellerID)
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
            if (!sellerID.HasValue)
            {
                throw new ArgumentException("Seller ID cannot be null.");
            }
        }

        private async Task<bool> IsValidCategoryAsync(int categoryID)
        {
            // Fetch all categories asynchronously from the repository
            var validCategories = await _categoryRepo.GetAllAsync();

            // Check if any of the categories have the given categoryID
            return validCategories.Any(category => category.CategoryId == categoryID);
        }
    }

}

