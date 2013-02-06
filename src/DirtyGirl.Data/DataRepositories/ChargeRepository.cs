using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class ChargeRepository : Repository<CartCharge>, IChargeRepository
    {
        public ChargeRepository(DB context) : base(context) { }
    }
}
