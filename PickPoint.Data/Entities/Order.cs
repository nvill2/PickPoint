using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PickPoint.Data.Entities
{
    public class Order
    {
        public Order()
        {
        }

        public Order(int number, string items, decimal amount, string customerName, string customerPhone, OrderStatus status, string deliveryPointNumber = null)
        {
            Items = items;
            Amount = amount;
            CustomerName = customerName;
            CustomerPhone = customerPhone;
            Number = number;
            Status = status;
            DeliveryPointNumber = deliveryPointNumber;
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Number { get; set; }

        public OrderStatus Status { get; private set; } = OrderStatus.Registered;
        
        public string Items { get; set; }

        [NotMapped]
        public string[] ItemList => Items?.Split("|", StringSplitOptions.RemoveEmptyEntries);

        [Required]
        public decimal Amount { get; set; }

        [ForeignKey(nameof(DeliveryPoint))]
        public string DeliveryPointNumber { get; set; }

        [Required]
        public string CustomerPhone { get; set; }

        [Required]
        public string CustomerName { get; set; }

        public DeliveryPoint DeliveryPoint { get; set; }

        public void Cancel() => Status = OrderStatus.Cancelled;
    }
}
