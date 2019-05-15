using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class CategoryBindingModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int HouseholdId { get; set; }
    }
}