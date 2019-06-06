using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Household_Budgeter.Models
{
    public class EditTransactionBindingModel
    {
        //[Required]
        public int BankAccountId { get; set; }
        //public SelectList BankAccount { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        //[Required]
        public int CategoryId { get; set; }
        public bool IsOwner { get; set; }
        //public SelectList Category { get; set; }
    }
}