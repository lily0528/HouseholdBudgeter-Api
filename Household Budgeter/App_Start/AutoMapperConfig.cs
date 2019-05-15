using AutoMapper;
using Household_Budgeter.Models;
using Household_Budgeter.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Household_Budgeter.App_Start
{
    public class AutoMapperConfig
    {

        public static void Init()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<Household, HouseholdBindingModel>().ReverseMap();
                cfg.CreateMap<Household, HouseholdView>().ReverseMap();
                cfg.CreateMap<Invitation, InvitationBindingModel>().ReverseMap();
                cfg.CreateMap<Invitation, InvitationView>().ReverseMap();
                cfg.CreateMap<Category, CategoryBindingModel>().ReverseMap();
                cfg.CreateMap<Category, CategoryView>().ReverseMap();
            });
        }
    }
}