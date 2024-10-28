using BusinessObject.Base;
using CloudinaryDotNet;
using Common;
using Data;
using Data.Models;
using Data.Utils;
using Data.Utils.HashPass;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        Task<IBusinessResult> GetByEmailOrUserNameAndPasswordAsync(string email, string password);
        Task<IBusinessResult> GetByEmailAsync(string email);
        Task<User?> GetFromCookie(HttpRequest request);
        Task<IBusinessResult> Register(User cate, string confirmationLink);
        Task<User?> GetUserByToken(string token);
        Task<IBusinessResult> ReadOnlyActiveSellers();
        Task<User?> GetUserByHashedToken(string token);
        Task<IBusinessResult> ChangeEmail(User cate, string newEmail);
        /*        void UpdateCookie(HttpRequest request, HttpResponse response);
        */
    }

    public class UserBusiness : IUserBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        public UserBusiness(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<string> GenerateRandomPasswordResetTokenByEmailAsync(string email)
        {
            Random random = new Random();
            var token = "";
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < 8; i++)
            {
                token += chars[random.Next(chars.Length)];
            }

            return Task.FromResult(token);
        }
        public async Task<IBusinessResult> GetByEmailOrUserNameAndPasswordAsync(string email, string password)
        {
            try
            {
                var result = await _unitOfWork.UserRepository.GetByEmailOrUserNameAndPasswordAsync(email, HashPassWithSHA256.HashWithSHA256(password));
                if (result != null)
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

        public async Task<IBusinessResult> ReadOnlyActiveSellers()
        {
            try
            {
                #region Business rule
                #endregion

                var sellers = await _unitOfWork.UserRepository.GetListWithNoTracking(u => u.IsActive == true && u.Products.Count > 0);


                if (sellers == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA__MSG);
                }
                else
                {
                    return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, sellers);
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
                cate.PasswordHash = HashPassWithSHA256.HashWithSHA256(cate.PasswordHash);
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
        public async Task<IBusinessResult> ChangeEmail(User cate, string newEmail)
        {
            try
            {
                cate.Token = Guid.NewGuid().ToString();
                cate.Email = newEmail;
                cate.IsActive = false;
                int result = await _unitOfWork.UserRepository.Update(cate);

                if (result > 0)
                {
                    var confirmationLink =
                    $"https://localhost:7141/confirm?token={cate.Token}";
                    //  $"https://secondsoul2nd.azurewebsites.net/confirm?token={cate.Token}"; //deploy
                    var emailSend = await SendMail.SendConfirmationEmail(cate.Email, confirmationLink);

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

        public async Task<IBusinessResult> Update(User cate)
        {
            try
            {
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

        public async Task<User?> GetFromCookie(HttpRequest request)
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
                        return user;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        /*        public void UpdateCookie(HttpRequest request,HttpResponse response)
                {
                    try
                    {

                        #region Business rule
                        #endregion

                        var currentUser = request.Cookies["User"];
                        if (!string.IsNullOrEmpty(currentUser))
                        {   
                            response.Cookies.Delete("User");
                            var userJson = JsonSerializer.Serialize(currentUser);

                            var cookieOptions = new CookieOptions
                            {
                                Expires = DateTime.Now.AddDays(30),
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict
                            };

                            response.Cookies.Append("User", userJson, cookieOptions);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
        */
        public async Task<IBusinessResult> GetByEmailAsync(string email)
        {
            try
            {
                var result = await _unitOfWork.UserRepository.GetByEmailAsync(email.Trim().ToLower());
                if (result == null)
                {
                    return new BusinessResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
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

        public async Task<IBusinessResult> Register(User cate, string confirmationLink)
        {
            try
            {
                cate.PasswordHash = HashPassWithSHA256.HashWithSHA256(cate.PasswordHash);
                int result = await _unitOfWork.UserRepository.CreateAsync(cate);
                if (result > 0)
                {
                    //var confirmationLink =
                    //$"https://localhost:7141/Confirm?token={cate.Token}";
                    //  $"https://secondsoul2nd.azurewebsites.net/confirm?token={cate.Token}"; //deploy
                    var emailSend = await SendMail.SendConfirmationEmail(cate.Email, confirmationLink);
                    if (!emailSend)
                    {
                        Console.WriteLine("Email send error");
                    }
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
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
        public async Task<User?> GetUserByToken(string token)
        {
            return await _unitOfWork.UserRepository.GetUserByToken(token);
        }
        public async Task<User?> GetUserByHashedToken(string token)
        {
            var list = await _unitOfWork.UserRepository.GetListWithNoTracking(u => !string.IsNullOrEmpty(u.Token));
            return list.SingleOrDefault(u => !string.IsNullOrEmpty(u.Token) && HashPassWithSHA256.HashWithSHA256(u.Token) == token);
        }
    }
}