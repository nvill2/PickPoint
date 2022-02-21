using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PickPoint.Data.Entities;

namespace PickPoint.Core.Services
{
    public class DeliveryPointRepository : EfRepository<DeliveryPoint>
    {
        public DeliveryPointRepository(DbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<DeliveryPoint>> GetAsync(Func<DeliveryPoint, bool> func)
        {
            return await Task.Run(() => this.dbSet.Include("Orders").Where(func).ToArray());
        }
    }
}
