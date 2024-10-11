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
        public async Task<int> CreateOrderAsync(int customerId, List<int> productIds, string phoneNumber, string address, int totalAmount, int? couponId)
        {
            // Create a new Order
            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = "Pending",  // Initial status
                PhoneNumber = phoneNumber,
                Address = address,
                CouponId = couponId
            };

            // Add the order to the context
            context.Orders.Add(order);
            await context.SaveChangesAsync();  // Save the order to generate OrderId

            // Add OrderDetails for each product
            foreach (var productId in productIds)
            {
                var product = await context.Products.FindAsync(productId);
                if (product != null)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = productId,
                        Price = product.Price  // Set the product price
                    };
                    context.OrderDetails.Add(orderDetail);
                }
            }

            // Save all the order details
            await context.SaveChangesAsync();
            return order.OrderId;
        }

    }
}
