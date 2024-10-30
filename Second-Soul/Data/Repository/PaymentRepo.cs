using Data.Base;
using Data.Models;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class PaymentRepo : GenericRepo<Payment>
    {
        private readonly PayOS _payOS;
        public PaymentRepo(SecondSoulShopContext context,PayOS payOS) : base(context)
        {
            _payOS = payOS;
        }
    }
}
