using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class HouseholdBindingModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
   
    }
}