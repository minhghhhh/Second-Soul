using BusssinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.OrderService
{
    public interface IOrderService
    {
        Task<Order?> GetOrder(int OrderId);
        Task<Order?> ReadOnlyOrder(int OrderId);
        Task<List<Order>> ReadOnlyOrdersByUserId(int userId);
        Task AddOrder(Order order);
    }
}
