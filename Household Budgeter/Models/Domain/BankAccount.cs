﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public decimal Balance { get; set; }
        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
        public BankAccount()
        {
            Transactions = new List<Transaction>();
        }
    }
}