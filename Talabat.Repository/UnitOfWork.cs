using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Repository.Data;
using Talabat.Repository.Repositories;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _context;
        private Hashtable _repositories;
        public UnitOfWork(StoreDbContext context)
        {
            _context = context;
            _repositories = new Hashtable();
        }
        public async Task<int> CompleteAsync()=>  await _context.SaveChangesAsync();

        public ValueTask DisposeAsync()=> _context.DisposeAsync();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var key = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(key))
            {
                var repository = new GenericRepository<TEntity>(_context);
                _repositories.Add(key, repository);
            }
            return _repositories[key] as IGenericRepository<TEntity>;
        }
    }
}
