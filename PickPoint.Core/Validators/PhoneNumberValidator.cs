using PickPoint.Core.Contracts;
using PickPoint.Data.Entities;
using System.Text.RegularExpressions;

namespace PickPoint.Core.Validators
{
    public class PhoneNumberValidator : IOrderValidator
    {
        public bool Validate(Order order)
        {
            return Regex.IsMatch(order.CustomerPhone, @"\+7\d{3}-\d{3}-\d{2}-\d{2}");
        }
    }
}
