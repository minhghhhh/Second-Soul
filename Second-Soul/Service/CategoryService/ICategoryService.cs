using BusssinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CategoryService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
    }
}
