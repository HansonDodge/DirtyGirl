using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Data.DataRepositories
{
    public class CartDiscountItemRepository: Repository<CartDiscountItem>, ICartDiscountItemRepository
    {
        public CartDiscountItemRepository(DB dbContext) : base(dbContext) { }
    }
}
