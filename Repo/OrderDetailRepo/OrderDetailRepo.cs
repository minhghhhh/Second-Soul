using BusssinessObject;
using Repo.GenericRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.OrderDetailRepo
{
    public class OrderDetailRepo : GenericRepo<OrderDetail>, IOrderDetailRepo
    {
        public OrderDetailRepo(SecondSoulShopContext context) : base(context)
        {
        }
    }
}
