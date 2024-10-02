using BusssinessObject;
using Microsoft.Identity.Client;
using Repo.CategoryRepo;
using Repo.ProductRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Service.ProductService
{
    public class ProductService : IProductService
    { private readonly IProductRepo _productRepo;
        private readonly ICategoryRepo _categoryRepo;
        public ProductService(IProductRepo productRepo, ICategoryRepo categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }
        public async Task<List<Product>> SearchProduct(string query, decimal? minPrice, decimal? maxPrice, int? categoryID, string condition, bool? isAvailable, long? sellerID)
        {
            try
            {
                // Validate the parameters asynchronously
                await ValidateSearchParametersAsync(minPrice, maxPrice, categoryID, isAvailable, sellerID);

                // Call the repository method if all validations pass
                return await _productRepo.SearchProduct(query, minPrice, maxPrice, categoryID, condition, isAvailable, sellerID);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"An error occurred in ProductService: {ex.Message}");

                // Optionally rethrow or return an empty list
                return new List<Product>();
            }
        }

        private async Task ValidateSearchParametersAsync(decimal? minPrice, decimal? maxPrice, int? categoryID, bool? isAvailable, long? sellerID)
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
            if (categoryID.HasValue && !(await IsValidCategoryAsync(categoryID.Value)))
            {
                throw new ArgumentException("Invalid category ID.");
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

        // Example validation for category (this would usually call a service or database to check if the category exists)
        private async Task<bool> IsValidCategoryAsync(int categoryID)
        {
            // Fetch all categories asynchronously from the repository
            var validCategories = await _categoryRepo.GetAllAsync();

            // Check if any of the categories have the given categoryID
            return validCategories.Any(category => category.CategoryId == categoryID);
        }

    }
}
