using PickPoint.Data.Entities;

namespace PickPoint.Core.Contracts
{
    public interface IOrderValidator
    {
        bool Validate(Order order);
    }
}
