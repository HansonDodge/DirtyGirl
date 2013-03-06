using DirtyGirl.Data.DataInterfaces;
using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;
using System.Linq;

namespace DirtyGirl.Data.DataRepositories
{
    public class UserRepository:Repository<User>, IUserRepository
    {
        public UserRepository(DB context) : base(context) { }

        public User Get(int id)
        {
            return All().SingleOrDefault(x => x.UserId == id);
        }
    }
}
