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
        List<Product> SortPriceHighToLow(List<Product> products);
        List<Product> SortPriceLowToHigh(List<Product> products);
        List<Product> SortNewestProduct(List<Product> products);
        List<Product> SortOldestProduct(List<Product> products);
        Task<List<Product>> GetAllSellProduct();
        Task<List<Product>> GetProductPriceHighToLow();
        Task<List<Product>?> GetRelatedProduct(Product product);
        Task<List<Product>> GetProductPriceLowToHigh();
        Task<List<Product>> GetOldestProducts();
        Task<List<Product>> GetNewestProducts();
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int id);
        Task<IBusinessResult> Save(Product cate);
        Task<IBusinessResult> Update(Product cate);
        Task<IBusinessResult> DeleteById(int id);
        Task<bool> IdExists(int id);
        Task<List<Product>> GetProductsBySeller(int id);
        Task<List<Product>> GetFilterdAccountProduct(DateTime fromDate, DateTime toDate, int accountId);
        IQueryable<List<Product>> GetProductsAsQueryable();
        Task<IBusinessResult> SearchProduct(string? query, int? minPrice, int? maxPrice, List<int>? categoryIDs, string? condition, string? size, bool? isAvailable, int? sellerID);

    }
    public class ProductBusiness : IProductBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        public ProductBusiness(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Product>> GetFilterdAccountProduct(DateTime fromDate, DateTime toDate, int accountId)
        {
            return await _unitOfWork.ProductRepository.GetFilterdAccountProduct(fromDate, toDate, accountId);
        }
        public async Task<List<Product>> GetProductsBySeller(int id)
        {
            return await _unitOfWork.ProductRepository.GetProductsBySeller(id);
        }
        public async Task<List<Product>> GetOldestProducts()
        {
            return await _unitOfWork.ProductRepository.GetProductOldest();
        }
        public async Task<List<Product>> GetNewestProducts()
        {
            return await _unitOfWork.ProductRepository.GetProductsNewest();
        }
        public async Task<List<Product>> GetAllSellProduct()
        {
            return await _unitOfWork.ProductRepository.GetAllSellProduct();
        }
        public async Task<List<Product>> GetProductPriceHighToLow()
        {
            return await _unitOfWork.ProductRepository.GetProductPriceHighToLow();
        }
        public async Task<List<Product>> GetProductPriceLowToHigh()
        {
            return await _unitOfWork.ProductRepository.GetProductPriceLowToHigh();
        }


        public IQueryable<List<Product>> GetProductsAsQueryable()
        {
            return _unitOfWork.ProductRepository.GetProductsAsQueryable();
        }

        public async Task<List<Product>?> GetRelatedProduct(Product product)
        {
            return await _unitOfWork.ProductRepository.GetRelatedProduct(product);
        }
        public async Task<IBusinessResult> SearchProduct(string? query, int? minPrice, int? maxPrice, List<int>? categoryIDs, string? condition,string? size, bool? isAvailable, int? sellerID)
        {
            try
            {
                #region Business rule
                #endregion
                if (!string.IsNullOrEmpty(query))
                {
                    query = FormatUtilities.TrimSpacesPreserveSingle(query);
                }
                var search = await _unitOfWork.ProductRepository.SearchProduct(query, minPrice, maxPrice, categoryIDs, condition,size, isAvailable, sellerID);
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
                var cv = await _unitOfWork.ProductRepository.GetProductDetails(id);

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
                var cvid = await _unitOfWork.ProductRepository.GetByIdAsync(id);
                if (cvid != null)
                {
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
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        public async Task<bool> IdExists(int id)
        {
            var cate = await _unitOfWork.ProductRepository.GetByIdAsync(id);
            return cate != null;
        }

        public List<Product> SortPriceHighToLow(List<Product> products)
        {
            return _unitOfWork.ProductRepository.SortPriceHighToLow(products);
        }

        public List<Product> SortPriceLowToHigh(List<Product> products)
        {
            return _unitOfWork.ProductRepository.SortPriceLowToHigh(products);
        }

        public List<Product> SortNewestProduct(List<Product> products)
        {
            return  _unitOfWork.ProductRepository.SortNewestProduct(products);
        }

        public  List<Product>SortOldestProduct(List<Product> products)
        {
            return _unitOfWork.ProductRepository.SortOldestProduct(products);
        }
    }
}
