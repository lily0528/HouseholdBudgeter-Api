using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set;}
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }
    }
}