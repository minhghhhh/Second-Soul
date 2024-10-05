using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Base;
using Common;
using Data;
using Data.Models;
using Data.Repository;
using System.Xml.Linq;


namespace BusinessObject
{
	public interface ICategoryBusiness
	{
		Task<IBusinessResult> GetAll();
		Task<IBusinessResult> GetById(int id);
		Task<IBusinessResult> Save(Category cate);
		Task<IBusinessResult> Update(Category cate);
		Task<IBusinessResult> DeleteById(int id);
		Task<bool> IdExists(int id);
		Task<IBusinessResult> AdvancedSearch(int? id, string name, int? parentid);

		// Này mẫu từ project trước của tui th nên có xem qua nếu xài nha
	}

	public class CategoryBusiness : ICategoryBusiness
	{
		private readonly UnitOfWork _unitOfWork;
		public CategoryBusiness(UnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IBusinessResult> GetAll()
		{
			try
			{
				#region Business rule
				#endregion

				//var currencies = _DAO.GetAll();
				//var currencies = await _currencyRepository.GetAllAsync();
				var categories = await _unitOfWork.CategoryRepository.GetAllAsync();


				if (categories == null || categories.Count == 0)
				{
					return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
				}
				else
				{
					return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, categories);
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
				var cv = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

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

		public async Task<IBusinessResult> Save(Category cate)
		{
			try
			{
				//int result = await _currencyRepository.CreateAsync(currency);
				int result = await _unitOfWork.CategoryRepository.CreateAsync(cate);
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

		public async Task<IBusinessResult> Update(Category cate)
		{
			try
			{
				//int result = await _currencyRepository.UpdateAsync(currency);
				int result = await _unitOfWork.CategoryRepository.Update(cate);

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
				var cvid = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
				if (cvid != null)
				{
					//var result = await _currencyRepository.RemoveAsync(currency);
					var result = await _unitOfWork.CategoryRepository.RemoveAsync(cvid);
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
			var cate = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
			return cate != null;
		}








		//public async Task<IBusinessResult> SearchByName(string name)
		//{
		//    try
		//    {
		//        var cvs = await _unitOfWork.CategoryRepo.GetAllAsync();
		//        var filteredCvs = cvs.Where(c => c.InternName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

		//        if (filteredCvs.Any())
		//        {
		//            return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, filteredCvs);
		//        }
		//        else
		//        {
		//            return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
		//    }
		//}

		//public async Task<IBusinessResult> SearchBySchoolName(string schoolName)
		//{
		//    try
		//    {
		//        var cvs = await _unitOfWork.CategoryRepo.GetAllAsync();
		//        var filteredCvs = cvs.Where(c => c.SchoolName.Contains(schoolName, StringComparison.OrdinalIgnoreCase)).ToList();

		//        if (filteredCvs.Any())
		//        {
		//            return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, filteredCvs);
		//        }
		//        else
		//        {
		//            return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
		//    }
		//}

		public async Task<IBusinessResult> AdvancedSearch(int? id, string name, int? parentid)
		{
			try
			{
				var cvs = await _unitOfWork.CategoryRepository.GetAllAsync();
				var filteredCvs = cvs.Where(c =>
					(!id.HasValue || c.CategoryId == id.Value) &&
					(string.IsNullOrEmpty(name) || c.CategoryName.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
					(!id.HasValue || c.ParentCategoryId == id.Value)
				).ToList();

				if (filteredCvs.Any())
				{
					return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, filteredCvs);
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
	}
}
