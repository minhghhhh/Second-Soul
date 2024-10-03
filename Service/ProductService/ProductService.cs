using BusssinessObject;
using Microsoft.Identity.Client;
using Repo.CategoryRepo;
using Repo.ProductRepo;
using Service.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Service.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<List<Product>> GetallProduct()
        {
            return await _unitOfWork.ProductRepository.GetAllAsync();
        }
        public async Task<List<Product>> SearchProduct(string query, decimal? minPrice, decimal? maxPrice, List<int>? categoryIDs, string condition, bool? isAvailable, long? sellerID, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                // Validate the parameters asynchronously
                await ValidateSearchParametersAsync(minPrice, maxPrice, categoryIDs, isAvailable, sellerID);

                // Call the repository method if all validations pass
                return await _unitOfWork.ProductRepository.SearchProduct(query, minPrice, maxPrice, categoryIDs, condition, isAvailable, sellerID, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"An error occurred in ProductService: {ex.Message}");

                // Optionally rethrow or return an empty list
                return new List<Product>();
            }
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

        // Example validation for category (this would usually call a service or database to check if the category exists)
        private async Task<bool> IsValidCategoryAsync(int categoryID)
        {
            // Fetch all categories asynchronously from the repository
            var validCategories = await _unitOfWork.CategoryRepository.GetAllAsync();

            // Check if any of the categories have the given categoryID
            return validCategories.Any(category => category.CategoryId == categoryID);
        }

    }
}
