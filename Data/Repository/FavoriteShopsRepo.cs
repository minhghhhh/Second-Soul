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
    public class FavoriteShopsRepo : GenericRepo<FavoriteShop>
    {
        public FavoriteShopsRepo(SecondSoulShopContext context) : base(context)
        {
        }
        public async Task<List<FavoriteShop>> GetAllUserFavouriteShop(int id)
        {
            return await context.FavoriteShops.Where(f => f.UserId==id).ToListAsync();    
        }
    }
}
