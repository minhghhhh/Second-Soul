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
        public async Task<ShoppingCart?> GetByUserIdAndProductId(int userId, int productId)
        {
            return await _dbcontext.ShoppingCarts
                .SingleOrDefaultAsync(s => s.UserId == userId && s.ProductId == productId);
        }

        public async Task<int> GetTotalCartFromSelectedId(int userId,List<int> productIds)
        {
            var total = 0;
            if (userId == null) { throw new Exception("No User Found"); }
            if (userId < 0) { throw new Exception("UserId must be positive"); }
            foreach (var productId in productIds)
            {
              var cartItem =  await GetByUserIdAndProductId(userId, productId);
                if (cartItem != null)
                total += cartItem.Product.Price;
            }           
            return  total;
        }
        public async Task DeleteItemsFromCart(int userId, List<int> productIds)
        {
            if (userId < 0) { throw new Exception("UserId must be positive"); }
            foreach (var productId in productIds)
            {
                var cartItem = await GetByUserIdAndProductId(userId, productId);
                if (cartItem != null)
                    await Delete(cartItem);
            }
        }
    }
}
