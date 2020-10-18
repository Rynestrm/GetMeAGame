using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace COMP2084GetMeAGame.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}")]
        [Range(0.01, 999999)]
        public double Price { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string Photo { get; set; }
        public string Description { get; set; }

        // reference the parent class (1 Category - Many Products)
        public Category Category { get; set; }
    }
}
