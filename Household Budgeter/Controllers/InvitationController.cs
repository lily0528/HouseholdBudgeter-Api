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
        [Route("EmailInvitation")]
        //[Route("EmailInvitation/{email}")]
        public IHttpActionResult EmailInvitation(InvitationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var invitationUser = DbContext.Users.FirstOrDefault(p => p.Email == model.Email);
            var ifInvitation = DbContext.Invitations.FirstOrDefault(p => p.inviteeId == invitationUser.Id && p.HouseholdId== model.HouseholdId);
            if (invitationUser == null)
            {
                return BadRequest("This invitee isn't registed user!");
            }
            else if(ifInvitation != null)
            {
                return BadRequest("This invitee already invited!");
            }

            var household = DbContext.Households.FirstOrDefault(p => p.Id == model.HouseholdId);
            var ifHouseholdMember = DbContext.Households.Where(p => p.JoinedUsers.Any(m => m.Email == model.Email) && p.Id == model.HouseholdId).FirstOrDefault();
            if (household.CreatorId != userId)
            {
                return BadRequest("The creator isn't this household's owner!");
            }
            if (household == null)
            {
                return BadRequest("This household isn't existed!");
            }
            if (ifHouseholdMember != null)
            {
                return BadRequest("This invitee already joined!");
            }
           
            var invitation = Mapper.Map<Invitation>(model);
            invitation.inviteeId = invitationUser.Id;
            invitation.Created = DateTime.Now;
            invitation.OwnerId = userId;
            DbContext.Invitations.Add(invitation);
            DbContext.SaveChanges();
            var emailService = new EmailService();
            emailService.Send(model.Email, $"The owner of household invite you join householdId: {household.Id} household Name: {household.Name}", "Join Household");
            return Ok();
        }

        [HttpPost]
        [Route("AcceptInvitation/{id:int}")]
        public IHttpActionResult AcceptInvitation(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.Find(userId);
            var household = DbContext.Households.FirstOrDefault(p => p.Id == id);
            if (household == null)
            {
                return BadRequest("Your hosehold id is wrong!");
            }

            var ifHouseholdMember = DbContext.Households.FirstOrDefault(p => p.JoinedUsers.Any(m => m.Email == user.Email) && p.Id == id);
            if (ifHouseholdMember != null)
            {
                return BadRequest("You already joined household!");
            }

            var invitation = DbContext.Invitations.FirstOrDefault(p => p.HouseholdId == id && p.inviteeId == userId);
            if(invitation == null)
            {
                return BadRequest("You are not invited!");
            }

            household.JoinedUsers.Add(invitation.invitee);
            DbContext.Invitations.Remove(invitation);
            DbContext.SaveChanges();
            return Ok();
        }
    }
}

