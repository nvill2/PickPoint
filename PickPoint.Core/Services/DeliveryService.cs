using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PickPoint.Core.Contracts;
using PickPoint.Data;
using PickPoint.Data.Entities;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace PickPoint.Core.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly OrderRepository orderRepository;
        private readonly DeliveryPointRepository deliveryPointRepository;
        private readonly List<IOrderValidator> validators;

        private readonly RedLockFactory redLockFactory;

        public DeliveryService(OrderRepository orderRepository,
            DeliveryPointRepository deliveryPointRepository,
            List<IOrderValidator> validators)
        {
            this.orderRepository = orderRepository;
            this.deliveryPointRepository = deliveryPointRepository;
            this.validators = validators;

            var endPoints = new List<RedLockEndPoint>
            {
                new DnsEndPoint("localhost", 6379)
            };
            this.redLockFactory = RedLockFactory.Create(endPoints);
        }
        public async Task<OperationResult> CancelOrder(int number)
        {
            using (var redLock = this.redLockFactory.CreateLock($"{nameof(this.CancelOrder)}_{number}", TimeSpan.FromSeconds(10)))
            {
                try
                {
                    var orderToCancel = (await this.orderRepository.GetAsync(o => o.Number == number)).First();

                    if (!redLock.IsAcquired)
                    {
                        return OperationResult.ServerError;
                    }

                    orderToCancel.Cancel();
                    var result = await this.orderRepository.SaveAsync();

                    return GetDataOperationResult(result);
                }
                catch (InvalidOperationException)
                {
                    return OperationResult.NotFound;
                }
                catch (Exception)
                {
                    return OperationResult.ServerError;
                }
            }
        }

        public async Task<OperationResult> CreateOrder(Order order)
        {
            using (var redLock = this.redLockFactory.CreateLock($"{nameof(this.CreateOrder)}_{order.CustomerPhone}", TimeSpan.FromSeconds(10)))
            {
                try
                {
                    if (!redLock.IsAcquired)
                    {
                        return OperationResult.Locked;
                    }

                    if (!ValidateOrder(order))
                    {
                        return OperationResult.ValidationFailed;
                    }

                    var deliveryPoint =
                        (await this.deliveryPointRepository.GetAsync(d => d.Number == order.DeliveryPointNumber))
                        .FirstOrDefault();

                    if (deliveryPoint == null)
                    {
                        return OperationResult.NotFound;
                    }

                    if (!deliveryPoint.Status)
                    {
                        return OperationResult.OutOfService;
                    }

                    order.DeliveryPoint = deliveryPoint;

                    await this.orderRepository.AddAsync(order);
                    var result = await this.orderRepository.SaveAsync();

                    return GetDataOperationResult(result);
                }
                catch (Exception)
                {
                    return OperationResult.ServerError;
                }
            }
        }

        public async Task<DeliveryPoint> GetDeliveryPoint(string number)
        {
            return (await this.deliveryPointRepository.GetAsync(d => d.Number.Equals(number, StringComparison.OrdinalIgnoreCase))).FirstOrDefault();
        }

        public async Task<Order> GetOrder(int number)
        {
            return (await this.orderRepository.GetAsync(o => o.Number == number)).FirstOrDefault();
        }

        public async Task<IEnumerable<DeliveryPoint>> GetWorkingDeliveryPoints()
        {
            return await this.deliveryPointRepository.GetAsync(d => d.Status == true);
        }

        public async Task<OperationResult> UpdateOrder(Order order)
        {
            using (var redLock = this.redLockFactory.CreateLock($"{nameof(this.CancelOrder)}_{order.Number}", TimeSpan.FromSeconds(10)))
            {
                try
                {
                    if (!redLock.IsAcquired)
                    {
                        return OperationResult.ServerError;
                    }

                    if (!ValidateOrder(order))
                    {
                        return OperationResult.ValidationFailed;
                    }

                    var orderToUpdate = (await this.orderRepository.GetAsync(o => o.Number == order.Number)).First();
                    UpdateOrderProperties(orderToUpdate, order);

                    var result = await this.orderRepository.SaveAsync();

                    return GetDataOperationResult(result);
                }
                catch (InvalidOperationException)
                {
                    return OperationResult.NotFound;
                }
                catch (Exception)
                {
                    return OperationResult.ServerError;
                }
            }
        }

        private void UpdateOrderProperties(Order orderToUpdate, Order order)
        {
            orderToUpdate.Items = order.Items;
            orderToUpdate.CustomerName = orderToUpdate.CustomerName;
            orderToUpdate.Amount = order.Amount;
            orderToUpdate.CustomerPhone = order.CustomerPhone;
            orderToUpdate.DeliveryPoint = order.DeliveryPoint;
        }

        private OperationResult GetDataOperationResult(int result)
        {
            return result <= 0 ? OperationResult.ServerError : OperationResult.Success;
        }

        private bool ValidateOrder(Order order)
        {
            foreach (var validator in this.validators)
            {
                if (!validator.Validate(order))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
