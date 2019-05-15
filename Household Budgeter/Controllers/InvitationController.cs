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
        //[Route("{email}")]
        //[Route("SendEmail/{email:string}")] todo: test
        [Route("SendEmail")]
        //public IHttpActionResult SendEmail(string email, InvitationBindingModel model)
        public IHttpActionResult SendEmail(InvitationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invitationUser = DbContext.Users.FirstOrDefault(p => p.Email == model.Email);
            var ifInvitation = DbContext.Invitations.FirstOrDefault(p => p.inviteeId == invitationUser.Id);
            if (invitationUser == null)
            {
                return BadRequest("This invitee isn't registed user!");
            }
            else if(ifInvitation != null)
            {
                return BadRequest("This invitee already invited!");
            }

            var household = DbContext.Households.FirstOrDefault(p => p.Id == model.HouseholdId);
            var ifHouseholdMember = DbContext.Households.Where(p => p.JoinedUsers.Any(m => m.Email == model.Email)).FirstOrDefault();
            if (household == null)
            {
                return BadRequest("This household isn't existed!");
            }
            else if (ifHouseholdMember != null)
            {
                return BadRequest("This invitee already joined!");
            }
            
            var userId = User.Identity.GetUserId();
            var invitation = Mapper.Map<Invitation>(model);
            invitation.inviteeId = invitationUser.Id;
            invitation.Created = DateTime.Now;
            invitation.OwnerId = userId;
            DbContext.Invitations.Add(invitation);
            DbContext.SaveChanges();
            var emailService = new EmailService();
            emailService.Send(model.Email, $"Manager invite you join householdId: {household.Id} household: {household.Name}", "Join Household");
            var result = Mapper.Map<InvitationView>(invitation);
            return Ok(result);
        }

        [HttpPost]
        [Route("AcceptInvitation/{id:int}")]
        public IHttpActionResult AcceptInvitation(int id)
        {
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(p => p.Id == id);
            var invitation = DbContext.Invitations.FirstOrDefault(p => p.HouseholdId == id && p.inviteeId == userId);
            if(invitation == null)
            {
                return BadRequest("No invitation data");
            }
            household.JoinedUsers.Add(invitation.invitee);
            DbContext.SaveChanges();
            return Ok();
        }
    }
}

