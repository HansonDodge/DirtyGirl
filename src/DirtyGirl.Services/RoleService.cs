using System;
using System.Linq;
using System.Web.Security;
using DirtyGirl.Data;
using DirtyGirl.Data.DataInterfaces;
using Ninject;
using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Data.DataInterfaces.Repositories;

namespace DirtyGirl.Services
{
    public class RoleService : RoleProvider
    {
        [Inject]
        public IRepositoryGroup _repository { get; set; }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (string roleName in roleNames)
            {
                var role = _repository.Roles.Find(x => x.Name == roleName);
                foreach (var username in usernames)
                {
                    var user = _repository.Users.Find(user1 => user1.UserName == username);
                    if (user != null)
                    {
                        if (user.Roles.All(x => x.Name != roleName))
                        {
                            user.Roles.Add(role);
                        }
                    }
                }
                
            }
            _repository.SaveChanges();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            var roles = _repository.Roles.All().Select(x => x.Name).ToArray();
            return roles;
        }

        public override string[] GetRolesForUser(string username)
        {
            var user = _repository.Users.Find(x => x.UserName.ToLower() == username.ToLower());
            var roles = user.Roles.Select(x => x.Name).ToArray();
            return roles;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
