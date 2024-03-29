﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class TransactionBindingModel
    {
        [Required]
        public int BankAccountId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        [Required]
        public int CategoryId { get; set; }
        //public bool IfVoid { get; set; }

    }
}