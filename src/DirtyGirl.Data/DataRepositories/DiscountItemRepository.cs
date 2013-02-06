using DirtyGirl.Data.DataInterfaces.Repositories;
using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Data.DataRepositories
{
    public class DiscountItemRepository: Repository<DiscountItem>, IDiscountItemRepository
    {
        public DiscountItemRepository(DB context) : base(context) { }
    }
}
