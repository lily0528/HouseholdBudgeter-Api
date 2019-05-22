using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class TransactionView
    {
        public int Id { get; set; }
        public int BankAccountId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal amount { get; set; }
        public int CategoryId { get; set; }
        public string CreatorName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IfVoid { get; set; }
    }
}