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
    public class OrderRepo : GenericRepo<Order>
    {
        public OrderRepo(SecondSoulShopContext context) : base(context)
        {
        }
        public async Task<List<Order>> GetOrdersByUserID(int userID)
        {
            return await context.Orders.Include(a=>a.Coupon).Where(a=>a.CustomerId == userID).ToListAsync();
        }
        public async Task<int> CreateOrderAsync(int customerId, List<int> productIds, string fullname ,string phoneNumber, string address, int totalAmount, int? couponId)
        {
            // Create a new Order
            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                FullName = fullname,
                Status = "Pending",  // Initial status
                PhoneNumber = phoneNumber,
                Address = address,
                CouponId = couponId
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync(); 

            foreach (var productId in productIds)
            {
                var product = await context.Products.FindAsync(productId);
                if (product != null)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.ProductId = product.ProductId;
                    orderDetail.OrderId = order.OrderId;
                    if (product.IsSale && product.SalePrice != null && product.SalePrice < product.Price)
                    {
                        orderDetail.Price = (int)product.SalePrice;
                    }
                    else
                    {
                        product.IsSale = false;
                        product.SalePrice = null;
                        context.Products.Update(product);
                        orderDetail.Price = product.Price;
                    }
                    context.OrderDetails.Add(orderDetail);
                }
            }
            // Save all the order details
            await context.SaveChangesAsync();
            return order.OrderId;
        }
        public async Task<List<Order>> GetFilterdAccountOrder(DateTime fromDate, DateTime toDate, int accountId)
        {
            var filteredBills = context.Orders
                .Where(b => b.CustomerId == accountId && b.OrderDate >= fromDate && b.OrderDate <= toDate)
                .OrderBy(b => b.OrderDate)
                .ToList();
            return filteredBills;
        }
        public async Task<Order?> GetOrderWithOrderDetailsAndProductsById(int orderId)
        {
            return await context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).SingleOrDefaultAsync(o => o.OrderId == orderId);
        }

		public async Task<Order?> GetPendingOrderWithOrderDetailsAndProductsByUserId(int userId)
		{
			return await context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).SingleOrDefaultAsync(o => o.CustomerId == userId && o.Status == "Pending");
		}
	}
}
