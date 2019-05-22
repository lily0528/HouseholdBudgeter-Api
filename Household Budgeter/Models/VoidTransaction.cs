using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class VoidTransaction
    {
        public int TransactionId { get; set; }
        public bool IfVoid { get; set; }
    }
}