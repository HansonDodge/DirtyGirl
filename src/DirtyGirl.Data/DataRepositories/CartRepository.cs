using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(DB context) : base(context) { }
    }
}
