using Data.Base;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class ReviewRepo : GenericRepo<Review>
    {
        public ReviewRepo(SecondSoulShopContext context) : base(context)
        {
        }
    }
}
