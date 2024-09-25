using BusssinessObject;
using Repo.GenericRepo;
using Repo.ProductRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.CategoryRepo
{
    public class CategoryRepo : GenericRepo<Category>, ICategoryRepo
    {
        public CategoryRepo(SecondSoulShopContext context) : base(context)
        {
        }
    }
}
