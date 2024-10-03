using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusssinessObject
{ public interface IOrderDetailBusiness { }
    public class OrderDetailBusiness : IOrderDetailBusiness
    {
        /*

                public async Task<OrderDetail?> GetOrderDetail(int OrderDetailId)
                {
                    if (!(OrderDetailId > 0))
                    {
                        throw new Exception("Order cannot be found.");
                    }
                    return await _unitOfWork.OrderDetailRepository.GetByIdAsync(OrderDetailId);
                }

                public async Task<OrderDetail?> ReadOnlyOrderDetail(int OrderDetailId)
                {
                    if (!(OrderDetailId > 0))
                    {
                        throw new Exception("Order cannot be found.");
                    }
                    return await _unitOfWork.OrderDetailRepository.GetSingleOrDefaultWithNoTracking(o => o.OrderDetailId == OrderDetailId);
                }

                public async Task<List<OrderDetail>> ReadOnlyOrderDetailsByOrderId(int OrderId)
                {

                    if (!(OrderId > 0))
                    {
                        throw new Exception("Order Details cannot be found.");

                    }
                    return await _unitOfWork.OrderDetailRepository.GetListWithNoTracking(o => o.OrderId == OrderId);
                }
        */
    }
}
