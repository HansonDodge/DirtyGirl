using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    class RegistrationRepository : Repository<Registration>, IRegistrationRepository
    {
        public RegistrationRepository(DB context) : base(context) { }
    }
}
