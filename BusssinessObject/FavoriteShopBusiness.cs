using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusssinessObject
{ public interface IFavoriteShopBusiness
    {

    }
    public class FavoriteShopBusiness : IFavoriteShopBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        public FavoriteShopBusiness(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



    }
}
