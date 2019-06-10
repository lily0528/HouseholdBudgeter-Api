using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class HouseholdSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<BankAccountSummaryView> BankAccounts { get; set; }
        public IEnumerable<CategorySummaryView> Categories { get; set; }
    }

    
}