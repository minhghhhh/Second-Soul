using BusssinessObject;
using Repo.GenericRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.ProductRepo
{
    public interface IProductRepo : IGenericRepo<Product>
    {
        IQueryable<List<Product>> GetProductsAsQueryable();
        Task<List<Product>> SearchProduct(string query, decimal? minPrice, decimal? maxPrice, List<int>? categoryIDs, string condition, bool? isAvailable, long? sellerID, int pageIndex = 1, int pageSize = 10);

    }
}