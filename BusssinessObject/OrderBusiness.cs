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
using System.Net.WebSockets;

namespace BusssinessObject
{
	public interface IOrderBusiness
    {
		Task<List<Order>> GetOrdersByUserId(int userId);
        Task<IBusinessResult> GetById(int orderId);
		Task<IBusinessResult> ReadOnlyById(int orderId);
		Task<IBusinessResult> ReadOnlyOrdersByUserId(int userId);
		Task<IBusinessResult> Save(Order order);
		Task<IBusinessResult> Update(Order order);
		Task<IBusinessResult> GetPendingOrder(int orderId, int? userId);
		Task<IBusinessResult> GetSinglePendingOrder(int userId);
		Task<IBusinessResult> DeleteById(int id);
		Task<int> CreateOrderAsync(int customerId, List<int> productIds,string fullname ,string phoneNumber, string address, int totalAmount, int? couponId);

	}
	public class OrderBusiness : IOrderBusiness
	{
		private readonly UnitOfWork _unitOfWork;
		public OrderBusiness(UnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<int> CreateOrderAsync(int customerId, List<int> productIds, string fullname, string phoneNumber, string address, int totalAmount, int? couponId)
		{
			return await _unitOfWork.OrderRepository.CreateOrderAsync(customerId, productIds, fullname, phoneNumber, address, totalAmount, couponId);
		}
		public async Task<IBusinessResult> DeleteById(int id)
		{
			try
			{
				var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
				if (order != null)
				{
					var result = await _unitOfWork.OrderRepository.RemoveAsync(order);
					if (result)
					{
						return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
					}
					else
					{
						return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
					}
				}
				else
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
			}
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
		public async Task<IBusinessResult> GetSinglePendingOrder(int userId)
		{
			try
			{
				#region Business rule
				#endregion

				if (!(userId > 0))
				{
					throw new Exception("Order cannot be found.");
				}

				var order = await _unitOfWork.OrderRepository.GetPendingOrderWithOrderDetailsAndProductsByUserId(userId);

				if (order == null)
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
				}
				else
				{
					var result = await ValidatePendingOrder(order, userId);
					if (result.order == null || !string.IsNullOrEmpty(result.error))
					{
						return new BusinessResult(Const.FAIL_READ_CODE, !string.IsNullOrEmpty(result.error) ? result.error : Const.FAIL_READ_MSG);
					}
					return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result.order);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
			}

		}


		public async Task<IBusinessResult> GetPendingOrder(int orderId, int? userId)
		{
			try
			{
				#region Business rule
				#endregion

				if (!(orderId > 0))
				{
					throw new Exception("Order cannot be found.");
				}

				var order = await _unitOfWork.OrderRepository.GetOrderWithOrderDetailsAndProductsById(orderId);

				if (order == null)
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
				}
				else
				{
					var result = await ValidatePendingOrder(order, userId);
					if (result.order == null || !string.IsNullOrEmpty(result.error))
					{
						return new BusinessResult(Const.FAIL_READ_CODE, !string.IsNullOrEmpty(result.error) ? result.error : Const.FAIL_READ_MSG);
					}
					return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result.order);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
			}

		}
		public async Task<List<Order>> GetOrdersByUserId(int userId)
		{
			return await _unitOfWork.OrderRepository.GetOrdersByUserID(userId);
		}

		public async Task<(Order? order, System.String? error)> ValidatePendingOrder(Order? order, int? userId)
		{
			System.String? error = null;
			if (order != null)
			{
				if (order.Status != "Pending")
				{
					await _unitOfWork.OrderRepository.RemoveAsync(order);
					return (null, "This is not a pending order.");
				}
				if (userId != null && order.CustomerId != userId)
				{
					return (null, "This order cannot be accessed by any other customer but its own.");
				}
				var orderDetails = await _unitOfWork.OrderDetailRepository.GetListWithNoTracking(record => record.OrderId == order.OrderId);
				if (orderDetails == null || orderDetails.Count <= 0)
				{
					await _unitOfWork.OrderRepository.RemoveAsync(order);
					return (null, "This order contains no valid and available items.");
				}
				var errorMessage = new StringBuilder("Unavailable Product(s):");
				int i = 0;
				while (i < orderDetails.Count)
				{
					var orderDetail = await _unitOfWork.OrderDetailRepository.GetDetailByProductIdAndOrderId(orderDetails[i].OrderId, orderDetails[i].ProductId);
					if (orderDetail == null)
					{
						orderDetails.RemoveAt(i);
					}
					else if (orderDetail.Product == null || orderDetail.Product.IsAvailable == false)
					{
						errorMessage.Append(orderDetail.Product == null ? string.Empty : " " + orderDetail.Product.Name + ",");
						await _unitOfWork.OrderDetailRepository.RemoveAsync(orderDetail);
						orderDetails.RemoveAt(i);
					}
					else if (orderDetail.Product.SalePrice == null || orderDetail.Product.SalePrice >= orderDetail.Product.Price)
					{

						orderDetail.Product.IsSale = false;
						orderDetail.Product.SalePrice = null;
						await _unitOfWork.ProductRepository.UpdateAsync(orderDetail.Product);
						i++;
					}
					else if (orderDetail.Product.IsSale || orderDetail.Product.SalePrice < orderDetail.Product.Price)
					{
                        orderDetail.Product.IsSale = true;
                        await _unitOfWork.ProductRepository.UpdateAsync(orderDetail.Product);
                        orderDetail.Price = (int)orderDetail.Product.SalePrice;
						await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
						i++;
					}
                }
				if (orderDetails == null || orderDetails.Count <= 0)
				{
					await _unitOfWork.OrderRepository.RemoveAsync(order);
					return (null, "This order contains no valid and available items.");
				}
				if (errorMessage.Length > "Unavailable Product(s):".Length)
				{
					errorMessage[errorMessage.Length - 1] = '.';
					error += errorMessage.ToString();
				}
				if (order.CouponId != null && order.CouponId > 0)
				{
					var total = orderDetails.Sum(o => o.Price);
					var result = await _unitOfWork.CouponRepository.GetSingleOrDefaultWithNoTracking(c => c.CouponId == (int)order.CouponId && c.IsActive && c.ExpiryDate > DateTime.Now);
					if (result == null || total < result.MinSpendAmount)
					{
						order.CouponId = null;
						order.TotalAmount = total;
						await _unitOfWork.OrderRepository.UpdateAsync(order);
					}
				}
				await _unitOfWork.SaveChangeAsync();
			}
			return (order, error);
		}
	}
}
