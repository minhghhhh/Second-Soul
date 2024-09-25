using BusssinessObject;
using Repo.GenericRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.OrderRepo
{
    public class OrderRepo : GenericRepo<Order>, IOrderRepo
    {
        public OrderRepo(SecondSoulShopContext context) : base(context)
        {

        }
    }
}
