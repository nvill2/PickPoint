using System.Text.RegularExpressions;
using PickPoint.Core.Contracts;
using PickPoint.Data.Entities;

namespace PickPoint.Core.Validators
{
    public class DeliveryPointNumberValidator : IOrderValidator
    {
        public bool Validate(Order order)
        {
            return Regex.IsMatch(order.DeliveryPointNumber, @"\d{4}-\d{3}");
        }
    }
}
