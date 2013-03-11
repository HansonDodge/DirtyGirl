using System;
using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Services.Utils;

namespace DirtyGirl.Services
{
    public abstract class ServiceBase
    {
        #region private members

        protected readonly IRepositoryGroup _repository;
        protected bool _sharedRepository = false;

        #endregion

        public ServiceBase(IRepositoryGroup _repository, bool isShared)
        {
            this._repository = _repository;
            this._sharedRepository = isShared;
        }

    }
}
