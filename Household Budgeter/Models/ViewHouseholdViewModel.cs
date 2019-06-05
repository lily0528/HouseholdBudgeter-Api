using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class ViewHouseholdViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfUsers { get; set; }
        public bool IsOwner { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
 
    }
}