using Microsoft.EntityFrameworkCore;
using PickPoint.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickPoint.Core.Services
{
    public abstract class EfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class

    {
        protected readonly DbContext context;
        protected DbSet<TEntity> dbSet;

        protected EfRepository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                return 0;

            this.dbSet.Update(entity);
            return await SaveAsync();
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                return 0;

            this.dbSet.UpdateRange(entities);

            return await SaveAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null)
                return;

            await this.dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                return;

            await this.dbSet.AddRangeAsync(entities);
        }

        public async Task<int> SaveAsync()
        {
            return await this.context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(TEntity item)
        {
            this.context.Remove(item);
            return await this.context.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> func)
        {
            return await Task.Run(() => this.dbSet.Where(func).ToArray());
        }
    }
}
