using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickPoint.Core.Contracts
{
    public interface IRepository<T>
        where T : class
    {
        Task AddAsync(T item);

        Task AddRangeAsync(IEnumerable<T> items);

        Task<int> SaveAsync();

        Task<int> UpdateAsync(T item);

        Task<int> UpdateRangeAsync(IEnumerable<T> items);

        Task<int> DeleteAsync(T item);

        public Task<IEnumerable<T>> GetAsync(Func<T, bool> func);
    }
}
