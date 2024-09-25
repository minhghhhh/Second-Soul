using BusssinessObject;
using Repo.CategoryRepo;
using Repo.CouponRepo;
using Repo.OrderDetailRepo;
using Repo.OrderRepo;
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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SecondSoulShopContext _dbContext;
        private readonly IUserRepo _userRepo;
        private readonly IProductRepo _productRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly ICouponRepo _couponRepo;
        private readonly IReviewRepo _reviewRepo;
        private readonly IOrderDetailRepo _orderDetailRepo;


        public UnitOfWork(SecondSoulShopContext dbContext, IUserRepo userRepository,IProductRepo productRepo,IOrderRepo orderRepo,ICategoryRepo categoryRepo,ICouponRepo couponRepo,IReviewRepo reviewRepo,IOrderDetailRepo orderDetailRepo)
        {
            _dbContext = dbContext;
            _userRepo = userRepository;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _categoryRepo = categoryRepo;
            _couponRepo = couponRepo;
            _reviewRepo = reviewRepo;
            _orderDetailRepo = orderDetailRepo;

        }
        public IUserRepo UserRepository => _userRepo;
        public IProductRepo ProductRepository => _productRepo;
        public IOrderRepo OrderRepository => _orderRepo;
        public ICategoryRepo CategoryRepository => _categoryRepo;
        public ICouponRepo CouponRepository => _couponRepo;
        public IReviewRepo ReviewRepository => _reviewRepo;
        public IOrderDetailRepo OrderDetailRepository => _orderDetailRepo;

        public Task<int> SaveChangeAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

    }
}
