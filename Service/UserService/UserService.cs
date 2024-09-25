using BusssinessObject;
using BusssinessObject.Enum;
using BusssinessObject.Utils;
using Service.HashPass;
using Service.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Service.UserService
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService()
        {

        }

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        public async Task CreateUser(User user)
        {
            try
            {
                if (user == null || user.UserId != default || user.Role != default || user.CreatedDate != default || user.IsActive != default)
                {
                    throw new Exception("The account cannot be created.");
                }
                user.Username = FormatUtilities.TrimSpacesPreserveSingle(user.Username);
                user.Role = Enums.Role.Customer.ToString();
                user.CreatedDate = DateTime.Now;
                user.IsActive = true;
                await _unitOfWork.UserRepository.Update(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
