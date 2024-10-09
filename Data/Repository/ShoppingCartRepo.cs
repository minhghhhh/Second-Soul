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
    public class ShoppingCartRepo : GenericRepo<ShoppingCart>
    {
        private readonly SecondSoulShopContext _dbcontext;
        public ShoppingCartRepo(SecondSoulShopContext context) : base(context)
        {
            _dbcontext = context;
        }

        public async Task<List<ShoppingCart>> GetShoppingCartsWithProductByUserId(int userId, int? offset, int? limit)
        {
            if (offset != null && limit != null && offset >= 0 && limit > 0)
            {
                return await _dbcontext.ShoppingCarts
                    .Where(s => s.UserId == userId)
                    .Skip((int)offset)
                    .Take((int)limit)
                    .Include(s => s.Product)
                    .ToListAsync();
            }
            return await _dbcontext.ShoppingCarts.Where(s => s.UserId == userId).Include(s => s.Product).ToListAsync();
        }
    }
}
