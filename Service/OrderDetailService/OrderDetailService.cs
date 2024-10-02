using BusssinessObject;
using Service.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.OrderDetailService
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
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

    }
}
