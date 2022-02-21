using PickPoint.Core.Contracts;
using PickPoint.Data.Entities;

namespace PickPoint.Core.Validators
{
    public class OrderItemsCountValidator : IOrderValidator
    {
        public bool Validate(Order order)
        {
            return order.ItemList?.Length <= 10;
        }
    }
}
