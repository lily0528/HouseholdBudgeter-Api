using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class InviteViewModel
    {
        public int HouseholdId { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
    }
}