using Data.Base;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class UserRepo : GenericRepo<User>
    {
        public UserRepo(SecondSoulShopContext context) : base(context)
        {
        }
        public async Task<User> GetByEmailAndPasswordAsync(string username, string password)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Username == username && a.PasswordHash == password);
        }
    }
}
