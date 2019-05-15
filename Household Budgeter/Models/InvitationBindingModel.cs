using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class InvitationBindingModel
    {
        public string UserId { get; set; }
        public string OwnerId { get; set; }
        public int HouseholdId { get; set; }
    }
}