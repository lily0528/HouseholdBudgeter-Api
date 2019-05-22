using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public decimal Amount { get; set; }

        public bool IfVoid { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public int BankAccountId { get; set; }
        public virtual BankAccount BankAccount { get; set; }
    }
}