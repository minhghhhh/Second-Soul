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
        Task<IBusinessResult> DeleteById(int id);
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

		public async Task<IBusinessResult> DeleteById(int id)
		{
			try
			{
				//var currency = await _currencyRepository.GetByIdAsync(code);
				var cvid = await _unitOfWork.ShoppingCartRepository.GetByIdAsync(id);
				if (cvid != null)
				{
					//var result = await _currencyRepository.RemoveAsync(currency);
					var result = await _unitOfWork.ShoppingCartRepository.RemoveAsync(cvid);
					if (result)
					{
						return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
					}
					else
					{
						return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
					}
				}
				else
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
				}
			}
			catch (Exception ex)
			{
				return new BusinessResult(-4, ex.ToString());
			}
		}
	}
}
