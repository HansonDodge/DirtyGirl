using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Data.DataInterfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
    }    
}
