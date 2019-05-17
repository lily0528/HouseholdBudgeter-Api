using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models.Domain
{
    public class Household
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public virtual List<ApplicationUser> JoinedUsers { get; set; }
     
        public virtual List<Invitation> Invitations { get; set; }
        public virtual List<Category> Categories { get; set; }

        public Household()
        {
            JoinedUsers = new List<ApplicationUser>();
            Invitations = new List<Invitation>();
            Categories = new List<Category>();
        }
    }
}