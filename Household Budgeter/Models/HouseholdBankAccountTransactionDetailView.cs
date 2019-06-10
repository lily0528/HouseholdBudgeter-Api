using Household_Budgeter.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class HouseholdBankAccountTransactionDetailView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string CategoryName { get; set; }
        //public decimal CategoryAmount { get; set; }
        //public int CategoryId { get; set; }
        //public virtual Category Category { get; set; }
    }
}