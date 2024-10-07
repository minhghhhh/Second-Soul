using BusinessObject.Base;
using Common;
using Data;
using Data.Models;
using Data.Utils.HashPass;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusssinessObject
{
	public interface IUserBusiness
	{
		Task<IBusinessResult> GetAll();
		Task<IBusinessResult> GetById(int id);
		Task<IBusinessResult> Save(User cate);
		Task<IBusinessResult> Update(User cate);
		Task<IBusinessResult> DeleteById(int id);
		Task<bool> IdExists(int id);
		Task<IBusinessResult> GetByEmailAndPasswordAsync(string email, string password);

		//  Task<IBusinessResult> AdvancedSearch(int? id, string name, int? parentid);

		// Này mẫu từ project trước của tui th nên có xem qua nếu xài nha
	}

	public class UserBusiness : IUserBusiness
	{
		private readonly UnitOfWork _unitOfWork;
		public UserBusiness(UnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IBusinessResult> GetByEmailAndPasswordAsync(string email, string password)
		{
			try
			{
				var result = await _unitOfWork.UserRepository.GetByEmailAndPasswordAsync(email, HashPassWithSHA256.HashWithSHA256(password));
				if (result != null && result.IsActive)
				{
					return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
				}
				else
				{
					return new BusinessResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
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

				var currencies = await _unitOfWork.UserRepository.GetAllAsync();


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
				var cv = await _unitOfWork.UserRepository.GetByIdAsync(id);

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

		/*
                 public async Task<bool> ConfirmUserByUsernameAndPassword(string email, string password)
                {
                    try
                    {
                        var user = await _unitOfWork.UserRepository.GetSingleOrDefaultWithNoTracking(u => u.Email.ToLower() == email.ToLower() && u.PasswordHash == HashPassWithSHA256.HashWithSHA256(password));
                        if (user != null)
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    return false;
                }

         */
		public async Task<IBusinessResult> Save(User cate)
		{
			try
			{
				//int result = await _currencyRepository.CreateAsync(currency);
				int result = await _unitOfWork.UserRepository.CreateAsync(cate);
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

		public async Task<IBusinessResult> Update(User cate)
		{
			try
			{
				//int result = await _currencyRepository.UpdateAsync(currency);
				int result = await _unitOfWork.UserRepository.Update(cate);

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
				var cvid = await _unitOfWork.UserRepository.GetByIdAsync(id);
				if (cvid != null)
				{
					//var result = await _currencyRepository.RemoveAsync(currency);
					var result = await _unitOfWork.UserRepository.RemoveAsync(cvid);
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
			var cate = await _unitOfWork.UserRepository.GetByIdAsync(id);
			return cate != null;
		}

		public static IBusinessResult GetFromCookie(HttpRequest request)
		{
			try
			{

				#region Business rule
				#endregion

				var userJson = request.Cookies["User"];
				if (!string.IsNullOrEmpty(userJson))
				{
					var user = JsonSerializer.Deserialize<User>(userJson);
					if (user != null)
					{
						return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, user);
					}
				}
				return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
			}
			catch (Exception ex)
			{
				return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
			}
		}


	}
}
