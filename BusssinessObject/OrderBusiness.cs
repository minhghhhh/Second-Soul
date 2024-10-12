using Data.Models;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Base;
using Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Data.Repository;

namespace BusssinessObject
{
	public interface IOrderBusiness
	{
		Task<IBusinessResult> GetById(int orderId);
		Task<IBusinessResult> ReadOnlyById(int orderId);
		Task<IBusinessResult> ReadOnlyOrdersByUserId(int userId);
		Task<IBusinessResult> Save(Order order);
		Task<IBusinessResult> Update(Order order);
		Task<int> CreateOrderAsync(int customerId, List<int> productIds, string phoneNumber, string address, int totalAmount, int? couponId);

	}
	public class OrderBusiness : IOrderBusiness
	{
		private readonly UnitOfWork _unitOfWork;
		public OrderBusiness(UnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<int> CreateOrderAsync(int customerId, List<int> productIds, string phoneNumber, string address, int totalAmount, int? couponId)
		{
			return await _unitOfWork.OrderRepository.CreateOrderAsync(customerId, productIds, phoneNumber, address, totalAmount, couponId);
		}

		public async Task<IBusinessResult> GetById(int orderId)
		{
			try
			{
				#region Business rule
				#endregion

				if (!(orderId > 0))
				{
					throw new Exception("Order cannot be found.");
				}

				var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

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

		public async Task<IBusinessResult> ReadOnlyById(int orderId)
		{
			try
			{
				#region Business rule
				#endregion

				if (!(orderId > 0))
				{
					throw new Exception("Order cannot be found.");
				}

				var order = await _unitOfWork.OrderRepository.GetSingleOrDefaultWithNoTracking(o => o.OrderId == orderId);

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
		public async Task<IBusinessResult> ReadOnlyOrdersByUserId(int userId)
		{

			try
			{
				#region Business rule
				#endregion

				if (!(userId > 0))
				{
					throw new Exception("Orders of the user cannot be found.");
				}

				var orders = await _unitOfWork.OrderRepository.GetListWithNoTracking(o => o.CustomerId == userId);

				if (orders == null || orders.Count == 0)
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
				}
				else
				{
					return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, orders);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
			}
		}

		public async Task<IBusinessResult> Save(Order order)
		{
			try
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
				int result = await _unitOfWork.OrderRepository.CreateAsync(order);
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

		public async Task<IBusinessResult> Update(Order order)
		{
			try
			{
				//int result = await _currencyRepository.UpdateAsync(currency);
				int result = await _unitOfWork.OrderRepository.Update(order);

				if (result > 0)
				{
					return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
				}
				else
				{
					return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
			}
		}

		public async Task<Order?> ValidatePendingOrder(Order? order)
		{

			if (order != null)
			{
				if (order.Status != "Pending")
				{
					return null;
				}
				var orderDetails = await _unitOfWork.OrderDetailRepository.GetListWithNoTracking(o => o.OrderId == order.OrderId);
				if (orderDetails == null || orderDetails.Count <= 0)
				{
					await _unitOfWork.OrderRepository.RemoveAsync(order);
					return null;
				}
				if (order.CouponId != null && order.CouponId > 0)
				{
					var total = orderDetails.Sum(o => o.Price);
					var result = await _unitOfWork.CouponRepository.GetSingleOrDefaultWithNoTracking(c => c.CouponId == (int)order.CouponId && c.IsActive && c.ExpiryDate > DateTime.Now);
					if (result == null || total < result.MinSpendAmount)
					{
						order.CouponId = null;
						order.TotalAmount = total;
						await _unitOfWork.SaveChangeAsync();
					}

				}
			}  
			return order;
		}
	}
}
