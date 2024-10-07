using Data.Base;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class UserRepo : GenericRepo<User>
    {
        public UserRepo(SecondSoulShopContext context) : base(context)
        {
        }

        // Tìm user bằng email và mật khẩu
        public async Task<User?> GetByEmailAndPasswordAsync(string email, string password)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower() && a.PasswordHash == password);
        }

        // Tìm user bằng email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Tạo mới user
        public async Task<int> CreateAsync(User user)
        {
            await _dbSet.AddAsync(user);
            return await context.SaveChangesAsync();
        }
    }
}
