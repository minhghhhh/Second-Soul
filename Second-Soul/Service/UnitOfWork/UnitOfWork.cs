
using Data.Models;
using Repo.CategoryRepo;
using Repo.CouponRepo;
using Repo.OrderDetailRepo;
using Repo.OrderRepo;
using Repo.PaymentRepo;
using Repo.ProductRepo;
using Repo.ReviewRepo;
using Repo.UserRepo;
using Service.PaymentService;
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
        private readonly UserRepo _userRepo;
        private readonly ProductRepo _productRepo;
        private readonly OrderRepo _orderRepo;
        private readonly CategoryRepo _categoryRepo;
        private readonly CouponRepo _couponRepo;
        private readonly ReviewRepo _reviewRepo;
        private readonly OrderDetailRepo _orderDetailRepo;
        private readonly PaymentRepo _paymentRepo;


        public UnitOfWork(SecondSoulShopContext dbContext, UserRepo userRepository, ProductRepo productRepo, OrderRepo orderRepo, CategoryRepo categoryRepo, CouponRepo couponRepo, ReviewRepo reviewRepo, OrderDetailRepo orderDetailRepo, PaymentRepo paymentRepo)
        {
            _dbContext = dbContext;
            _userRepo = userRepository;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _categoryRepo = categoryRepo;
            _couponRepo = couponRepo;
            _reviewRepo = reviewRepo;
            _orderDetailRepo = orderDetailRepo;
            _paymentRepo = paymentRepo;

        }
        public IUserRepo UserRepository => _userRepo;
        public IProductRepo ProductRepository => _productRepo;
        public IOrderRepo OrderRepository => _orderRepo;
        public ICategoryRepo CategoryRepository => _categoryRepo;
        public ICouponRepo CouponRepository => _couponRepo;
        public IReviewRepo ReviewRepository => _reviewRepo;
        public IOrderDetailRepo OrderDetailRepository => _orderDetailRepo;
        public IPaymentRepo PaymentRepository => _paymentRepo;


        public Task<int> SaveChangeAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

    }
}
