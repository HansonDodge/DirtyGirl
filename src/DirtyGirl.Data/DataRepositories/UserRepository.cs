using System.Collections.Generic;

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

        public List<User> GetUsers(string firstName, string lastName, string userName, string emailAddress)
        {
            var users = All();
            if (!string.IsNullOrEmpty(firstName))
                users = users.Where(u => u.FirstName.StartsWith(firstName));

            if (!string.IsNullOrEmpty(lastName))
                users = users.Where(u => u.LastName.StartsWith(lastName));

            if (!string.IsNullOrEmpty(userName))
                users = users.Where(u => u.UserName.StartsWith(userName));

            if (!string.IsNullOrEmpty(emailAddress))
                users = users.Where(u => u.EmailAddress.StartsWith(emailAddress));

            return users.ToList();
        }
    }
}
