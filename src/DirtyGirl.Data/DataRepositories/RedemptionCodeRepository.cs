using DirtyGirl.Data.DataInterfaces;
using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class RedemptionCodeRepository : Repository<RedemptionCode>, IRedemptionCodeRepository
    {
        public RedemptionCodeRepository(DB context) : base(context) { }
    }
}
