using BusinessObject.Base;
using Common;
using Data;
using Data.Models;
using Data.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusssinessObject
{
    public interface IProductBusiness
    {
        Task<List<Product>> GetOldestProducts();
        Task<List<Product>> GetNewestProducts();
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int id);
        Task<IBusinessResult> Save(Product cate);
        Task<IBusinessResult> Update(Product cate);
        Task<IBusinessResult> DeleteById(int id);
        Task<bool> IdExists(int id);
        IQueryable<List<Product>> GetProductsAsQueryable();
        Task<IBusinessResult> SearchProduct(string? query, decimal? minPrice, decimal? maxPrice, List<int>? categoryIDs, string? condition, bool? isAvailable, long? sellerID, int pageIndex = 1, int pageSize = 10);

    }
    public class ProductBusiness : IProductBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        public ProductBusiness(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Product>> GetOldestProducts()
        {
            return await _unitOfWork.ProductRepository.GetProductOldest();
        }
        public async Task<List<Product>> GetNewestProducts()
        {
            return await _unitOfWork.ProductRepository.GetProductsNewest();
        }

        public IQueryable<List<Product>> GetProductsAsQueryable()
        {
            return _unitOfWork.ProductRepository.GetProductsAsQueryable();
        }
        public async Task<IBusinessResult> SearchProduct(string? query, decimal? minPrice, decimal? maxPrice, List<int>? categoryIDs, string? condition, bool? isAvailable, long? sellerID, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                #region Business rule
                #endregion
                if (!string.IsNullOrEmpty(query))
                {
                    query = FormatUtilities.TrimSpacesPreserveSingle(query);
                }
                var search = await _unitOfWork.ProductRepository.SearchProduct(query, minPrice, maxPrice, categoryIDs, condition, isAvailable, sellerID, pageIndex, pageSize);
                if (search.Any())
                {

                    return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, search);
                }
                else
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }

            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
        public async Task<IBusinessResult> GetAll()
        {
            try
            {
                #region Business rule
                #endregion

                var currencies = await _unitOfWork.ProductRepository.GetAllAsync();


                if (currencies == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                else
                {
                    return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, currencies);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> GetById(int id)
        {
            try
            {
                #region Business rule
                #endregion

                //var currency = await _currencyRepository.GetByIdAsync(code);
                var cv = await _unitOfWork.ProductRepository.GetByIdAsync(id);

                if (cv == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                else
                {
                    return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, cv);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Save(Product cate)
        {
            try
            {
                //int result = await _currencyRepository.CreateAsync(currency);
                int result = await _unitOfWork.ProductRepository.CreateAsync(cate);
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

        public async Task<IBusinessResult> Update(Product cate)
        {
            try
            {
                //int result = await _currencyRepository.UpdateAsync(currency);
                int result = await _unitOfWork.ProductRepository.Update(cate);

                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                }
                else
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(-4, ex.ToString());
            }
        }

        public async Task<IBusinessResult> DeleteById(int id)
        {
            try
            {
                //var currency = await _currencyRepository.GetByIdAsync(code);
                var cvid = await _unitOfWork.ProductRepository.GetByIdAsync(id);
                if (cvid != null)
                {
                    //var result = await _currencyRepository.RemoveAsync(currency);
                    var result = await _unitOfWork.ProductRepository.RemoveAsync(cvid);
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

        public async Task<bool> IdExists(int id)
        {
            var cate = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            return cate != null;
        }



    }
}
