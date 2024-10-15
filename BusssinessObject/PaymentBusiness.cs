using BusinessObject.Base;
using Common;
using Data;
using Data.Models;
using Data.Utils;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusssinessObject
{
	public interface IPaymentBusiness
	{
		Task<PaymentLinkInformation?> CancelOrder(int orderId);
		Task<IBusinessResult> CreatePaymentLink(int orderId, string cancelUrl, string successUrl, int? userId);
	}
	public class PaymentBusiness : IPaymentBusiness
	{
		private readonly PayOS _payOS;
		private readonly UnitOfWork _unitOfWork;
		private readonly IOrderBusiness _orderBusiness;

		public PaymentBusiness(PayOS payOS, UnitOfWork unitOfWork, IOrderBusiness orderBusiness)
		{
			_payOS = payOS;
			_unitOfWork = unitOfWork;
			_orderBusiness = orderBusiness;
		}
		public async Task<PaymentLinkInformation?> CancelOrder(int orderId)
		{
			try
			{
				PaymentLinkInformation cancelInfor = await _payOS.cancelPaymentLink(orderId);
				return cancelInfor;
			}
			catch
			{
				return null;
			}
		}
		public async Task<IBusinessResult> CreatePaymentLink(int orderId, string cancelUrl, string successUrl, int? userId)
		{
			try
			{
				var result = await _orderBusiness.GetPendingOrder(orderId, userId);
				if (result == null || !(result.Status > 0) || result.Data == null)
				{
					return new BusinessResult(Const.FAIL_CREATE_CODE, result == null || string.IsNullOrEmpty(result.Message) ? Const.FAIL_CREATE_MSG : result.Message);
				}
				var order = (Order)result.Data;
				if (order == null || order.OrderDetails == null || order.OrderDetails.Count == 0 || order.OrderDetails.ElementAt(0).Product == null)
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, "The order is invalid, either due to unavailable products being processed or other errors.");
				}
				List<ItemData> itemlist = [];

				foreach (var orderDetails in order.OrderDetails)
				{
					var item = new ItemData(orderDetails.Product.Name, 1, orderDetails.Price);
					itemlist.Add(item);
				}
				var paymentData = new PaymentData(orderId, order.TotalAmount, order.Descriptions, itemlist, cancelUrl, successUrl);
				var paymentResult = await _payOS.createPaymentLink(paymentData);
				if (paymentResult != null)
				{
					return new BusinessResult(Const.SUCCESS_CREATE_CODE, string.IsNullOrEmpty(result.Message) ? Const.SUCCESS_READ_MSG : result.Message, paymentResult);
				}
				else
				{
					return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
			}
		}

	}
}
