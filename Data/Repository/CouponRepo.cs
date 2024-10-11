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
		public async Task<(bool isSuccess, string message, int discount, int totalWithDiscount, int? couponId)> ApplyCouponAsync(string couponCode, int total)
		{
			if (string.IsNullOrEmpty(couponCode))
			{
				return (false, "No coupon code provided.", 0, total, null);
			}

			var coupon = await context.Coupons.FirstOrDefaultAsync(c =>
				c.Code == couponCode && c.IsActive && c.ExpiryDate > DateTime.Now);
			if (coupon == null)
			{
				return (false, "Invalid or expired coupon.", 0, total, null);
			}
			if (total < coupon.MinSpendAmount)
			{
				var deficit = coupon.MinSpendAmount - total;
				return (false, "You have to spend " + deficit + " more to use this coupon", 0, total, null);

			}

			decimal discountNoCeiling = (total * coupon.DiscountPercentage / 100);
			int discount = (int)Math.Ceiling(discountNoCeiling);
			discount = discount > coupon.MaxDiscount ? coupon.MaxDiscount : discount;
			int totalWithDiscount = total - discount;

			string message = $"Coupon applied! You saved {discount.ToString("C")}";

			return (true, message, discount, totalWithDiscount, coupon.CouponId);
		}
	}
}
