using BusssinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.UserService
{
    public interface IUserService
    {
        Task<bool> ConfirmUserByUsernameAndPassword(string email, string password);
        Task CreateUser(User user);
    }
}
