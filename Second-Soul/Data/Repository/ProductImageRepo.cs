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
    public class ProductImageRepo : GenericRepo<ProductImage>
    {
        public ProductImageRepo(SecondSoulShopContext context) : base(context)
        {
        }
        public async Task<List<ProductImage>> GetAllProductImageByProductId(int id)
        {
            return await context.ProductImages.Where(p => p.ProductId == id).ToListAsync();
        }
    }
}
