using System.Collections.Generic;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataInterfaces.Repositories
{
    public interface IUserRepository: IRepository<User>
    {
        User Get(int id);
        List<User> GetUsers(string firstName, string lastName, string userName, string emailAddress);
    }
}
