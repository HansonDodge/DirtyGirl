using DirtyGirl.Data.DataInterfaces;
using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class UserRepository:Repository<User>, IUserRepository
    {
        public UserRepository(DB context) : base(context) { }
    }
}
