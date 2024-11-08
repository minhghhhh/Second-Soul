﻿using BusinessObject.Base;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Common;
using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusssinessObject
{
    public interface IProductImageBusiness
    {
        Task<IBusinessResult> GetByProductId(int id);
        Task<IBusinessResult> Save(ProductImage cate);
        Task<IBusinessResult> Update(ProductImage cate);
        Task<IBusinessResult> DeleteById(int id);
        Task<string> UploadImageAsync(IFormFile file);
        Task<ProductImage?> GetById(int id);
	}
    public class ProductImageBusiness : IProductImageBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Cloudinary _cloudinary;
        public ProductImageBusiness(UnitOfWork unitOfWork, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Create a memory stream to read the file
            using (var stream = new MemoryStream())
            {
                // Copy the file to the memory stream
                await file.CopyToAsync(stream);
                stream.Position = 0; // Reset stream position to the beginning

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "products",
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<IBusinessResult> GetByProductId(int id)
        {
            try
            {
                #region Business rule
                #endregion

                var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                var cv = await _unitOfWork.ProductImageRepo.GetAllProductImageByProductId(product.ProductId);

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
        public async Task<IBusinessResult> DeleteById(int id)
        {
            try
            {
                #region Business rule
                #endregion

                //var currency = await _currencyRepository.GetByIdAsync(code);
                var productImage = await _unitOfWork.ProductImageRepo.GetByIdAsync(id);
                if (productImage == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);

                }
                var result =  await _unitOfWork.ProductImageRepo.Delete(productImage);

                if (result < 0)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                else
                {
                    return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }


        public async Task<IBusinessResult> Save(ProductImage cate)
        {
            try
            {
                //int result = await _currencyRepository.CreateAsync(currency);
                int result = await _unitOfWork.ProductImageRepo.CreateAsync(cate);
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

        public async Task<IBusinessResult> Update(ProductImage cate)
        {
            try
            {
                //int result = await _currencyRepository.UpdateAsync(currency);
                int result = await _unitOfWork.ProductImageRepo.Update(cate);

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

        public async Task<ProductImage?> GetById(int id)
        {
            try
            {
                return await _unitOfWork.ProductImageRepo.GetByIdAsync(id);
            }
            catch
            {
                return null;
            }
        }
    }
}
