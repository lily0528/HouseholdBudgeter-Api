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
    [RoutePrefix("api/Invitation")]
    public class InvitationController : ApiController
    {
        private ApplicationDbContext DbContext;

        public InvitationController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("sendEmail")]
        public IHttpActionResult SendEmail(string email, InvitationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var invitation = Mapper.Map<Invitation>(model);
            invitation.Created = DateTime.Now;
            invitation.OwnerId = userId;
            DbContext.Invitations.Add(invitation);
            DbContext.SaveChanges();
            var result = Mapper.Map<HouseholdView>(invitation);
            return Ok(result);
        }
    }
}
