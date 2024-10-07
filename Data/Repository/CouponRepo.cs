using Data.Base;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class CouponRepo : GenericRepo<Coupon>
    {
        public CouponRepo(SecondSoulShopContext context) : base(context)
        {
        }
        public async Task DisableExpiredCoupons()
        {
            var expiredCoupons = context.Coupons
                .Where(c => c.ExpiryDate <= DateTime.Now && c.IsActive);

            foreach (var coupon in expiredCoupons)
            {
                coupon.IsActive = false;
            }

            await context.SaveChangesAsync();
        }
    }
}
