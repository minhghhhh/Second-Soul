using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Enum
{
    public static class Enums
    { public enum Role
        {
            Customer,
            Admin
        }
        public enum OrderStatus
        {
            Pending,
            Shipped,
            Delivered,
            Cancelled,
            Returned
        }
        public enum Condition
        {
            New,
            Like_New,
            Good,
            Fair 
        }

        public enum PaymentMethod
        {
            COD,
            Banking
        }
        public enum PaymentStatus
        {
            Pending,
            Completed,
            Failed
        }
    }
}
