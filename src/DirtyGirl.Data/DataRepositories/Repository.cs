using DirtyGirl.Data.DataInterfaces.Repositories;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DirtyGirl.Data.DataRepositories
{
    public class Repository<TObject> : IRepository<TObject>
            where TObject : class
    {
        private bool shareContext = false;

        public Repository()
        {
            Context = new DB();
        }

        public Repository(DB context)
        {
            Context = context;
            shareContext = true;
        }

        protected DB Context = null;

        protected DbSet<TObject> DbSet
        {
            get
            {
                return Context.Set<TObject>();
            }
        }

        public void Dispose()
        {
            if (shareContext && (Context != null))
                Context.Dispose();
        }

        public virtual IQueryable<TObject> All()
        {
            return DbSet.AsQueryable();
            
        }
        
        public virtual IQueryable<TObject> Filter(Expression<Func<TObject, bool>> predicate)
        {
           return DbSet.Where(predicate).AsQueryable<TObject>();
        }

        public virtual IQueryable<TObject> Filter(Expression<Func<TObject, bool>> predicate,IQueryable<string> includes)
        {
            var whereResults = DbSet.Where(predicate);
            foreach (string inc in includes)
            {
                whereResults.Include(inc);
            }
            return DbSet.Where(predicate).AsQueryable<TObject>();
        }

        public virtual IQueryable<TObject> Filter(Expression<Func<TObject, bool>> filter, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var _resetSet = filter != null ? DbSet.Where(filter).AsQueryable() : DbSet.AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        public bool Contains(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        public virtual TObject Find(params object[] keys)
        {
            return DbSet.Find(keys);
        }

        public virtual TObject Find(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual TObject Create(TObject TObject)
        {
            var newEntry = DbSet.Add(TObject);
            if (!shareContext)
            {
                Context.SaveChanges();
                Context.Entry(TObject).Reload();
            }
            return newEntry;
        }

        public virtual int Count
        {
            get
            {
                return DbSet.Count();
            }
        }

        public virtual int Delete(TObject TObject)
        {
            DbSet.Remove(TObject);
            if (!shareContext)
                return Context.SaveChanges();
            return 0;
        }

        public virtual int Update(TObject TObject)
        {
            var entry = Context.Entry(TObject);
            DbSet.Attach(TObject);
            entry.State = EntityState.Modified;
            if (!shareContext)
                return Context.SaveChanges();
            return 0;
        }

        public virtual int Delete(Expression<Func<TObject, bool>> predicate)
        {
            var objects = Filter(predicate);
            foreach (var obj in objects)
                DbSet.Remove(obj);
            if (!shareContext)
                return Context.SaveChanges();
            return 0;
        }

        public virtual void LoadProperties(TObject TObject)
        {
            Context.Entry(TObject).Reload();
        }

    }   

}
