using BusssinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.OrderDetailService
{
    public interface IOrderDetailService
    {
        Task<OrderDetail?> GetOrderDetail(int OrderDetailId);
        Task<OrderDetail?> ReadOnlyOrderDetail(int OrderDetailId);
        Task<List<OrderDetail>> ReadOnlyOrderDetailsByOrderId(int OrderId);
    }
}
