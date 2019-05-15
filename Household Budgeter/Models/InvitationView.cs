using Household_Budgeter.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class InvitationView
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }

        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public int HouseholdId { get; set; }
        public virtual Household Household { get; set; }
    }
}