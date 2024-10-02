using BusssinessObject;
using Microsoft.EntityFrameworkCore;
using Repo.GenericRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.ProductRepo
{
    public class ProductRepo :  GenericRepo<Product> , IProductRepo
    {
        private readonly SecondSoulShopContext _dbcontext;
        public ProductRepo(SecondSoulShopContext context) : base(context)
        {
            _dbcontext = context;
        }
        public async Task<List<Product>> SearchProduct(string query, decimal? minPrice, decimal? maxPrice, int? categoryID, string condition, bool? isAvailable, long? sellerID)
        {
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

            // Apply category filter
            if (categoryID.HasValue)
            {
                productQuery = productQuery.Where(p => p.CategoryId == categoryID.Value);
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
            return await productQuery.ToListAsync();
        }
    }
}
