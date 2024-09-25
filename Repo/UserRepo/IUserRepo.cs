using BusssinessObject;
using Repo.GenericRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.UserRepo
{
    public interface IUserRepo : IGenericRepo<User>
    {
    }
}
