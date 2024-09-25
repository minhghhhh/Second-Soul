using BusssinessObject;
using Repo.GenericRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.ProductRepo
{
    public class ProductRepo :  GenericRepo<Product> , IProductRepo
    {
        public ProductRepo(SecondSoulShopContext context) : base(context)
        {
        }
    }
}
