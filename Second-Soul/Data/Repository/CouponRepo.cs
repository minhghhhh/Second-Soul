using Data.Base;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

		public async Task<Coupon?> GetActiveCouponByCode(string code)
		{
			return await context.Coupons.SingleOrDefaultAsync(c =>
				c.Code == code && c.IsActive && c.ExpiryDate > DateTime.Now);
		}
		public async Task<(bool isSuccess, string message, int discount, int? couponId)> ApplyCouponAsync(string couponCode, int orderId)
		{
			if (string.IsNullOrEmpty(couponCode))
			{
				return (false, "No coupon code provided.", 0, null);
			}

			var order = await context.Orders.Include(o => o.OrderDetails).SingleOrDefaultAsync(record => record.OrderId == orderId);
			if (order == null)
			{
				return (false, "The order with the given id does not exist.", 0, null);
			}

			if (order.Status != "Pending")
			{
				return (false, "The order cannot be changed.", 0, null);
			}

			if (order.OrderDetails == null || order.OrderDetails.Count <= 0) 
			{
				context.Orders.Remove(order);
				await context.SaveChangesAsync();
				return (false, "The order is invalid.", 0, null);
			}
			
			var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.Code.ToLower().Trim() == couponCode.ToLower().Trim() && c.IsActive && c.ExpiryDate > DateTime.Now);
			if (coupon == null)
			{
				return (false, "Invalid or expired coupon.", 0, null);
			}


			var total = order.OrderDetails.Sum(o => o.Price);
			if (total < coupon.MinSpendAmount)
			{
				var deficit = coupon.MinSpendAmount - total;
				return (false, "You have to spend " + deficit + " more to use this coupon", 0, null);

			}

			decimal discountNoCeiling = (total * coupon.DiscountPercentage / 100);
			int discount = (int)Math.Ceiling(discountNoCeiling);
			discount = discount > coupon.MaxDiscount ? coupon.MaxDiscount : discount;
			int totalWithDiscount = total - discount;
			order.CouponId = coupon.CouponId;
			order.TotalAmount = totalWithDiscount;

			context.Orders.Update(order);
			await context.SaveChangesAsync();
			string message = $"Coupon applied! You saved {coupon.DiscountPercentage}%";

			return (true, message, discount, coupon.CouponId);
		}
	}
}
