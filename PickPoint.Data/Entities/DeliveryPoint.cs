using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace PickPoint.Data.Entities
{
    [Index(nameof(Number), IsUnique = true)]
    [Index(nameof(Status))]
    public class DeliveryPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Number { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public bool Status { get; set; }
        
        public virtual List<Order> Orders { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
