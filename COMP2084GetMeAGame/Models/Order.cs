using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace COMP2084GetMeAGame.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public double Total { get; set; }

        // child ref
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
