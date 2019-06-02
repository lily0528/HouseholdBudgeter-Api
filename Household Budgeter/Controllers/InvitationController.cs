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
    //[RoutePrefix("api/Invitation")]
    public class InvitationController : ApiController
    {
        private ApplicationDbContext DbContext;

        public InvitationController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpPost]
        //[Route("EmailInvitation")]
        public IHttpActionResult EmailInvitation(InvitationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(p => p.Id == model.HouseholdId);
            var ifHouseholdMember = DbContext.Households.Where(p => p.JoinedUsers.Any(m => m.Email == model.Email) && p.Id == model.HouseholdId).FirstOrDefault();
            if (household == null)
            {
                return BadRequest("The household is invalid!");
            }
            if (household.CreatorId != userId || ifHouseholdMember != null)
            {
                return BadRequest("The creator isn't this household's owner,or the information is wrong!");
            }

            var invitationUser = DbContext.Users.FirstOrDefault(p => p.Email == model.Email);
            var ifInvitation = DbContext.Invitations.FirstOrDefault(p => p.InviteeId == invitationUser.Id && p.HouseholdId == model.HouseholdId);
            if (invitationUser == null)
            {
                return BadRequest("This invitee isn't registed user!");
            }
            if (ifInvitation != null)
            {
                return BadRequest("This invitee already invited!");
            }

            var invitation = Mapper.Map<Invitation>(model);
            invitation.InviteeId = invitationUser.Id;
            invitation.Created = DateTime.Now;
            //invitation.OwnerId = userId;
            DbContext.Invitations.Add(invitation);
            DbContext.SaveChanges();
            var emailService = new EmailService();
            emailService.Send(model.Email, $"The owner of household invite you join householdId: {household.Id} household Name: {household.Name}", "Join Household");
            return Ok();
        }

        [HttpPost]
        //[Route("AcceptInvitation/{id:int}")]
        public IHttpActionResult AcceptInvitation(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.Find(userId);
            var invitation = DbContext.Invitations.FirstOrDefault(p => p.HouseholdId == id && p.InviteeId == userId);
            if (invitation == null)
            {
                return BadRequest("You are not invited!");
            }

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

            household.JoinedUsers.Add(invitation.Invitee);
            DbContext.Invitations.Remove(invitation);
            DbContext.SaveChanges();
            return Ok();
        }
    }
}

