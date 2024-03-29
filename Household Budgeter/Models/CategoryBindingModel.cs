﻿using Household_Budgeter.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Household_Budgeter.Models
{
    public class CategoryBindingModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        //public virtual Household Household { get; set; }
        public int HouseholdId { get; set; }
    }
}