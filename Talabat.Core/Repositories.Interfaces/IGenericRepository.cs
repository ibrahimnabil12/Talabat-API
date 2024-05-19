using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {

        Task<T?> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();



        Task<T?> GetWithSpecAsync(ISpecifications<T> spec);
        Task<IReadOnlyList<T>> GetWithSpecAllAsync(ISpecifications<T> spec);
        Task<int> GetCountAsync(ISpecifications<T> spec);

        Task AddAsync(T item);
        void Delete (T item);
        void Update(T item);
    }
}
