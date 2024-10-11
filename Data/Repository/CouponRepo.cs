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
		public async Task<(bool isSuccess, string message, int totalWithDiscount, int? couponId)> ApplyCouponAsync(string couponCode, int total)
		{
			if (string.IsNullOrEmpty(couponCode))
			{
				return (false, "No coupon code provided.", total, null);
			}

			var coupon = await context.Coupons.FirstOrDefaultAsync(c =>
				c.Code == couponCode && c.IsActive && c.ExpiryDate > DateTime.Now);

			if (coupon == null)
			{
				return (false, "Invalid or expired coupon.", total, null);
			}

			int discount = (int)(total * (coupon.DiscountPercentage));
			discount = discount > coupon.MaxDiscount ? coupon.MaxDiscount : discount;
			int totalWithDiscount = total - discount;

			string message = $"Coupon applied! You saved {discount.ToString("C")}";

			return (true, message, totalWithDiscount, coupon.CouponId);
		}
	}

}
}
