using BusssinessObject;
using Repo.GenericRepo;
using Repo.OrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.PaymentRepo
{
    public class PaymentRepo : GenericRepo<Payment>, IPaymentRepo
    {
        public PaymentRepo(SecondSoulShopContext context) : base(context)
        {

        }
    }
}
