using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class CouponRepository: Repository<Coupon>, ICouponRepository
    {
        public CouponRepository(DB context) : base(context) { }
    }
}
