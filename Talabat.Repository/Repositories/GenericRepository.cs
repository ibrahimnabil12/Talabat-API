using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;
using Talabat.Repository.Specifications;

namespace Talabat.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _context;

        public GenericRepository(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
          return await  _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<int> GetCountAsync(ISpecifications<T> spec)
        {
           return await SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(), spec).CountAsync();
        }

        public async Task<IReadOnlyList<T>> GetWithSpecAllAsync(ISpecifications<T> spec)
        {
            return await SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(),spec).ToListAsync();
        }

        public Task<T?> GetWithSpecAsync(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(), spec).FirstOrDefaultAsync();
        }

        public void Update(T item) => _context.Set<T>().Update(item);
        public async Task AddAsync(T item) => await _context.Set<T>().AddAsync(item);

        public void Delete(T item) => _context.Set<T>().Remove(item);
    }
}
