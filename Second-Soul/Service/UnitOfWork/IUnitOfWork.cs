using Repo.CategoryRepo;
using Repo.CouponRepo;
using Repo.OrderDetailRepo;
using Repo.OrderRepo;
using Repo.PaymentRepo;
using Repo.ProductRepo;
using Repo.ReviewRepo;
using Repo.UserRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IUserRepo UserRepository { get; }
        public IProductRepo ProductRepository { get; }
        public IOrderRepo OrderRepository { get; }
        public IOrderDetailRepo OrderDetailRepository { get; }  
        public ICategoryRepo CategoryRepository { get; }
        public ICouponRepo CouponRepository { get; }
        public IReviewRepo ReviewRepository { get; }
        public IPaymentRepo PaymentRepository { get; }
        public Task<int> SaveChangeAsync();
    }
}
