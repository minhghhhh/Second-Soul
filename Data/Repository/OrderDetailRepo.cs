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
	public class OrderDetailRepo : GenericRepo<OrderDetail>
	{
		private readonly CouponRepo _couponRepo;
		public OrderDetailRepo(SecondSoulShopContext context, CouponRepo couponRepo) : base(context)
		{
			_couponRepo = couponRepo;
		}
		public async Task<List<OrderDetail>> GetDetailsByOrderId(int orderId)
		{
			return await context.OrderDetails.Where(a => a.OrderId == orderId).ToListAsync();
		}
		public async Task<OrderDetail?> GetDetailByProductIdAndOrderId(int orderId, int productId)
		{
			return await context.OrderDetails.SingleOrDefaultAsync(record => record.OrderId == orderId && record.ProductId == productId);
		}
		public async Task<bool> RemoveOrderDetailAsync(OrderDetail orderDetail)
		{
			if (orderDetail != null)
			{
				var orderId = orderDetail.OrderId;
				context.OrderDetails.Remove(orderDetail);
				if (orderId > 0)
				{
					var order = await context.Orders.Include(record => record.OrderDetails).SingleOrDefaultAsync(record => record.OrderId == orderId);
					if (order != null)
					{
						if (order.OrderDetails.Count == 0)
						{
							context.Orders.Remove(order);
						}
						else
						{
							order.TotalAmount = order.OrderDetails.Sum(o => o.Price);
							if (order.CouponId != null)
							{
								var coupon = await context.Coupons.SingleOrDefaultAsync(record => record.CouponId == order.CouponId && record.IsActive && record.ExpiryDate > DateTime.Now);
								if (coupon == null || coupon.MinSpendAmount > order.TotalAmount)
								{
									order.CouponId = null;
								}
								else
								{
									await _couponRepo.ApplyCouponAsync(coupon.Code, orderId);
								}
							}
							context.Orders.Update(order);
						}
					}
				}
				return await context.SaveChangesAsync() > 0;
			}
			return false;
		}
	}
}
