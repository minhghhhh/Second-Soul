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
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
 
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task CreateUser(User user)
        {
            try
            {
                if (user == null || !(user.UserId > 0) || (string.IsNullOrEmpty(user.Role) || user.Role != "Customer" || user.Role != "") || user.CreatedDate != default || user.IsActive != default)
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
