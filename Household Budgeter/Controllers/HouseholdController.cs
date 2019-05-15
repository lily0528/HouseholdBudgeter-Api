using AutoMapper;
using Household_Budgeter.Models;
using Household_Budgeter.Models.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Household_Budgeter.Controllers
{
    [Authorize]
    [RoutePrefix("api/Household")]
    public class HouseholdController : ApiController
    {
        private ApplicationDbContext DbContext;

        public HouseholdController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("create")]
        public IHttpActionResult Create(HouseholdBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var household = Mapper.Map<Household>(model);
            household.Created = DateTime.Now;
            household.CreatorId = userId;
            //var household = new Household
            //{
            //    Name = model.Name,
            //    Description = model.Description,
            //};
            DbContext.Households.Add(household);
            DbContext.SaveChanges();
            var result = Mapper.Map<HouseholdView>(household);
            //var householdModel = new HouseholdView
            //{
            //    Id = household.Id,
            //    Name = household.Name,
            //    Description = household.Description,
            //    Created = household.Created,
            //    CreatorId = household.CreatorId,
            //    Creator = household.Creator
            //};
            return Ok(result);
        }

        [HttpPost]
        [Route("{id}")]
        public IHttpActionResult Edit(int id, HouseholdBindingModel model)
        {
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(p => p.Id == id && p.CreatorId == userId);
            if (household == null)
            {
                return NotFound();
            }
            Mapper.Map(model, household);
            DbContext.SaveChanges();
            var householdModel = Mapper.Map<HouseholdView>(household);
            return Ok(householdModel);
        }
    }
}
