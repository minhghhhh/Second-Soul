using BusssinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ProductService
{
    public interface IProductService
    {
        Task<List<Product>> SearchProduct(string query, decimal? minPrice, decimal? maxPrice, int? categoryID, string condition, bool? isAvailable, long? sellerID);
    }
}
