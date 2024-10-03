using Data.Models;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusssinessObject
{public interface IOrderBusiness { }
    public class OrderBusiness : IOrderBusiness
    {
/*        public async Task<Order?> GetOrder(int OrderId)
        {
            if (!(OrderId > 0))
            {
                throw new Exception("Order cannot be found.");
            }
            return await _unitOfWork.OrderRepository.GetByIdAsync(OrderId);
        }

        public async Task<Order?> ReadOnlyOrder(int OrderId)
        {
            if (!(OrderId > 0))
            {
                throw new Exception("Order cannot be found.");
            }
            return await _unitOfWork.OrderRepository.GetSingleOrDefaultWithNoTracking(o => o.OrderId == OrderId);
        }

        public async Task<List<Order>> ReadOnlyOrdersByUserId(int userId)
        {

            if (!(userId > 0))
            {
                throw new Exception("User cannot be found.");

            }
            return await _unitOfWork.OrderRepository.GetListWithNoTracking(o => o.CustomerId == userId);
        }

        public async Task AddOrder(Order order)
        {
            if (order == null || order.OrderId != default)
            {
                throw new Exception("Order cannot be added.");
            }
            if (!(order.TotalAmount > 0))
            {
                throw new Exception("Order's total amount is not valid.");
            }
            if (string.IsNullOrEmpty(order.PhoneNumber) || string.IsNullOrEmpty(order.Address))
            {
                throw new Exception("Order's phone number or address is invalid.");
            }
            await _unitOfWork.OrderRepository.AddAsync(order);
        }
*/
    }
}
