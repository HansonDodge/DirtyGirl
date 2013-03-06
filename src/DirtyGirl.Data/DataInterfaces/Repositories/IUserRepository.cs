using DirtyGirl.Models;

namespace DirtyGirl.Data.DataInterfaces.Repositories
{
    public interface IUserRepository: IRepository<User>
    {
        User Get(int id);
    }
}
