using Data.Base;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class CategoryRepo : GenericRepo<Category>
    {
        public CategoryRepo(SecondSoulShopContext context) : base(context)
        {
        }


    }
}
