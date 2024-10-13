using Data;
using Data.Models;
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
        Task<CreatePaymentResult> CreatePaymentLink(int orderId);
    }
    public class PaymentBusiness : IPaymentBusiness
    {
        private readonly PayOS _payOS;
private readonly UnitOfWork _unitOfWork;
        public PaymentBusiness(PayOS payOS,UnitOfWork unitOfWork )
        {
            _payOS = payOS;
            _unitOfWork = unitOfWork;
        }
        public async Task<PaymentLinkInformation?> CancelOrder(int orderId)
        {
            try
            {
                PaymentLinkInformation cancelInfor = await _payOS.cancelPaymentLink(orderId);
                return cancelInfor;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<CreatePaymentResult?> CreatePaymentLink(int orderId)
        {
            try
            {
                List<ItemData> itemlist = new List<ItemData>();
                var Order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                var orderDetails = await _unitOfWork.OrderDetailRepository.GetDetailsByOrderId(orderId);
                foreach (var order in orderDetails)
                {
                    var item = new ItemData(order.Product.Name, 1, order.Price);
                    itemlist.Add(item);
                }
                PaymentData paymentData = new PaymentData(orderId, Order.TotalAmount, Order.Descriptions, itemlist, "", "");
                return await _payOS.createPaymentLink(paymentData);
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
