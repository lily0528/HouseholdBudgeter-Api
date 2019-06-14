using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class ViewBankAccountView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Decimal Balance { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int HouseholdId { get; set; }
        public string HouseholdName { get; set; }
        public bool IsOwner { get; set; }
    }
}