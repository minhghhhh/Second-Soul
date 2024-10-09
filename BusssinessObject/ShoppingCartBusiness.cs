using BusinessObject.Base;
using Common;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusssinessObject
{
    public interface IShoppingCartBusiness
    {
        Task<IBusinessResult> GetByUserId(int userId, int? offset, int? limit);
        Task<IBusinessResult> Save(ShoppingCart cart);
        Task<IBusinessResult> RemoveFromCart(int userId, int productId);
    }

    public class ShoppingCartBusiness : IShoppingCartBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        public ShoppingCartBusiness(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IBusinessResult> GetByUserId(int userId, int? offset, int? limit)
        {
            try
            {
                #region Business rule
                #endregion

                if (offset != null && limit != null)
                {
                    if (offset < 0 || limit < 0)
                    {
                        return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                    }
                }

                var shoppingCarts = await _unitOfWork.ShoppingCartRepository.GetShoppingCartsWithProductByUserId(userId, offset, limit);


                if (shoppingCarts == null || shoppingCarts.Count == 0)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                else
                {
                    return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, shoppingCarts);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<IBusinessResult> Save(ShoppingCart cart)
        {
            try
            {
                //int result = await _currencyRepository.CreateAsync(currency);
                int result = await _unitOfWork.ShoppingCartRepository.CreateAsync(cart);
                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        public async Task<IBusinessResult> RemoveFromCart(int userId, int productId)
        {
            try
            {
                var cartItem = await _unitOfWork.ShoppingCartRepository.GetByUserIdAndProductId(userId, productId);
                if (cartItem == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

                var result = await _unitOfWork.ShoppingCartRepository.RemoveAsync(cartItem);

                if (result)
                {
                    return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
