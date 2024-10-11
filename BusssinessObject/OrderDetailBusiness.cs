using BusinessObject.Base;
using Common;
using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusssinessObject
{
	public interface IOrderDetailBusiness
	{
		Task<IBusinessResult> GetById(int orderDetailId);
		Task<IBusinessResult> ReadOnlyById(int orderDetailId);
		Task<IBusinessResult> ReadOnlyOrderDetailsByOrderId(int orderId);
		Task<IBusinessResult> Save(OrderDetail orderDetail);
		 Task<List<OrderDetail>> GetDetailsByOrderId(int orderId);

    }
    public class OrderDetailBusiness : IOrderDetailBusiness
	{

		private readonly UnitOfWork _unitOfWork;
		public OrderDetailBusiness(UnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
        public async Task<List<OrderDetail>> GetDetailsByOrderId(int orderId)
		{
			return await _unitOfWork.OrderDetailRepository.GetDetailsByOrderId(orderId);
		}

        public async Task<IBusinessResult> GetById(int orderDetailId)
		{
			try
			{
				#region Business rule
				#endregion

				if (!(orderDetailId > 0))
				{
					throw new Exception("Order cannot be found.");
				}

				var order = await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetailId);

				if (order == null)
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
				}
				else
				{
					return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, order);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
			}

		}

		public async Task<IBusinessResult> ReadOnlyById(int orderDetailId)
		{
			try
			{
				#region Business rule
				#endregion

				if (!(orderDetailId > 0))
				{
					throw new Exception("Order cannot be found.");
				}

				var order = await _unitOfWork.OrderDetailRepository.GetSingleOrDefaultWithNoTracking(o => o.OrderDetailId == orderDetailId);

				if (order == null)
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
				}
				else
				{
					return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, order);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
			}

		}

		public async Task<IBusinessResult> ReadOnlyOrderDetailsByOrderId(int orderId)
		{
			try
			{

				#region Business rule
				#endregion

				if (!(orderId > 0))
				{
					throw new Exception("Order Details of the order cannot be found.");
				}

				var orderDetails = await _unitOfWork.OrderDetailRepository.GetListWithNoTracking(o => o.OrderId == orderId);

				if (orderDetails == null || orderDetails.Count == 0)
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
				}
				else
				{
					return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orderDetails);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
			}
		}
		public async Task<IBusinessResult> Save(OrderDetail orderDetail)
		{
			try
			{
				if (orderDetail == null || orderDetail.OrderDetailId != default || orderDetail.OrderId == default || orderDetail.ProductId == default)
				{
					throw new Exception("Order Detail cannot be added.");
				}
				if (!(orderDetail.Price > 0))
				{
					throw new Exception("Order's price is not valid.");
				}
				int result = await _unitOfWork.OrderDetailRepository.CreateAsync(orderDetail);
				if (result > 0)
				{
					return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
				}
				else
				{
					return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
			}
		}


	}
}
