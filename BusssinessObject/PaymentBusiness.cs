using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusssinessObject
{public interface IPaymentBusiness { }
    public class PaymentBusiness : IPaymentBusiness
    {
        /*public async Task<Payment?> GetPayment(int PaymentId)
        {
            if (!(PaymentId > 0))
            {
                throw new Exception("Payment cannot be found.");
            }
            return await _unitOfWork.PaymentRepository.GetByIdAsync(PaymentId);
        }

        public async Task<Payment?> ReadOnlyPayment(int PaymentId)
        {
            if (!(PaymentId > 0))
            {
                throw new Exception("Payment cannot be found.");
            }
            return await _unitOfWork.PaymentRepository.GetSingleOrDefaultWithNoTracking(o => o.PaymentId == PaymentId);
        }*/

    }
}
