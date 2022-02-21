using PickPoint.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PickPoint.Core.Contracts
{
    public interface IDeliveryService
    {
        Task<IEnumerable<DeliveryPoint>> GetWorkingDeliveryPoints();

        Task<DeliveryPoint> GetDeliveryPoint(string number);

        Task<OperationResult> CreateOrder(Order order);

        Task<Order> GetOrder(int number);

        Task<OperationResult> UpdateOrder(Order order);

        Task<OperationResult> CancelOrder(int number);
    }
}
