using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;

namespace DirtyGirl.Data.DataRepositories
{
    public class PurchaseItemRepository: Repository<PurchaseItem>, IPurchaseItemRepository
    {
        public PurchaseItemRepository(DB context) : base(context){ }
    }
}
