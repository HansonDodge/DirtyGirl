using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class RoleRepository : Repository<Role>, DirtyGirl.Data.DataInterfaces.Repositories.IRoleRepository
    {
        public RoleRepository(DB context) : base(context) { }
    }        
}
