
using Data.Models;
using Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UnitOfWork 
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
        private readonly ShoppingCartRepo _shoppingCartRepo;
        private readonly ProductImageRepo _productImageRepo;
        private readonly FavoriteShopsRepo _favoriteShopRepo;

        public UnitOfWork(SecondSoulShopContext dbContext,FavoriteShopsRepo favoriteShopsRepo, ProductImageRepo productImageRepo,UserRepo userRepository, ProductRepo productRepo, OrderRepo orderRepo, CategoryRepo categoryRepo, CouponRepo couponRepo, ReviewRepo reviewRepo, OrderDetailRepo orderDetailRepo, PaymentRepo paymentRepo, ShoppingCartRepo shoppingCartRepo)
        {
            _productImageRepo = productImageRepo;
            _favoriteShopRepo = favoriteShopsRepo;
            _dbContext = dbContext;
            _userRepo = userRepository;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _categoryRepo = categoryRepo;
            _couponRepo = couponRepo;
            _reviewRepo = reviewRepo;
            _orderDetailRepo = orderDetailRepo;
            _paymentRepo = paymentRepo;
            _shoppingCartRepo = shoppingCartRepo;
        }
        public UserRepo UserRepository => _userRepo;
        public FavoriteShopsRepo FavoriteShopsRepo => _favoriteShopRepo;
        public ProductImageRepo ProductImageRepo => _productImageRepo;
        public ProductRepo ProductRepository => _productRepo;
        public OrderRepo OrderRepository => _orderRepo;
        public CategoryRepo CategoryRepository => _categoryRepo;
        public CouponRepo CouponRepository => _couponRepo;
        public ReviewRepo ReviewRepository => _reviewRepo;
        public OrderDetailRepo OrderDetailRepository => _orderDetailRepo;
        public PaymentRepo PaymentRepository => _paymentRepo;
		public ShoppingCartRepo ShoppingCartRepository => _shoppingCartRepo;
 
		public Task<int> SaveChangeAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

    }
}
