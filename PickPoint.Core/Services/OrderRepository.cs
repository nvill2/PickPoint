using Microsoft.EntityFrameworkCore;
using PickPoint.Data.Entities;

namespace PickPoint.Core.Services
{
    public class OrderRepository : EfRepository<Order>
    {
        public OrderRepository(DbContext context) : base(context)
        {
        }
    }
}
