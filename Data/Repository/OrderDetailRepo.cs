using Data.Base;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class OrderDetailRepo : GenericRepo<OrderDetail>
    {
        public OrderDetailRepo(SecondSoulShopContext context) : base(context)
        {
        }
        
    }
}
