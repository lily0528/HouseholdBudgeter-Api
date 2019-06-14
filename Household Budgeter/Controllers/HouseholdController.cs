using AutoMapper;
using AutoMapper.QueryableExtensions;
using Household_Budgeter.Models;
using Household_Budgeter.Models.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Household_Budgeter.Controllers
{
    [Authorize]
    //[RoutePrefix("api/Household")]
    public class HouseholdController : ApiController
    {
        private ApplicationDbContext DbContext;

        public HouseholdController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpGet]
        public IHttpActionResult GetHouseholdsSelectList()
        {
            var userId = User.Identity.GetUserId();
            var result = DbContext.Households.Where(p => p.CreatorId == userId)
              .Select(p => new ViewHouseholdViewModel
              {
                  Id = p.Id,
                  Name = p.Name,
                  NumberOfUsers = p.JoinedUsers.Count() + 1,
                  IsOwner = p.CreatorId == userId,
                  Description = p.Description,
                  Created = p.Created,
                  Updated = p.Updated
              }).ToList();

            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetHouseholds()
        {
            var userId = User.Identity.GetUserId();
            var result = DbContext.Households.Where(p => p.CreatorId == userId || p.JoinedUsers.Any(t => t.Id == userId))
              .Select(p => new ViewHouseholdViewModel
              {
                  Id = p.Id,
                  Name = p.Name,
                  NumberOfUsers = p.JoinedUsers.Count() + 1,
                  IsOwner = p.CreatorId == userId,
                  Description = p.Description,
                  Created = p.Created,
                  Updated = p.Updated
              }).ToList();

            return Ok(result);

        }

        [HttpPost]
        //[Route("create")]
        public IHttpActionResult Create(HouseholdBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var creator = DbContext.Users.Find(userId);
            if (creator == null)
            {
                return BadRequest("Can't find the user!");
            }
            var household = Mapper.Map<Household>(model);
            household.Created = DateTime.Now;
            household.CreatorId = userId;
            household.JoinedUsers.Add(creator);
            DbContext.Households.Add(household);
            DbContext.SaveChanges();
            var result = Mapper.Map<HouseholdView>(household);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(p => p.Id == id && p.CreatorId == userId);
            if (household == null)
            {
                return NotFound();
            }
            var model = Mapper.Map<HouseholdView>(household);
            return Ok(model);
        }

        [HttpPut]
        //[Route("{id}")]
        public IHttpActionResult Edit(int id, HouseholdBindingModel model)
        {
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(p => p.Id == id && p.CreatorId == userId);
            if (household == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (household.CreatorId != userId)
            {
                ModelState.AddModelError("", "It isn't creator of the household!");
                return BadRequest();
            }

            Mapper.Map(model, household);
            household.Updated = DateTime.Now;
            DbContext.SaveChanges();
            var householdModel = Mapper.Map<HouseholdBindingModel>(household);
            return Ok(householdModel);
        }


        [HttpDelete]
        //[Route("delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(p => p.Id == id && p.CreatorId == userId);
            if (household == null)
            {
                return BadRequest("Unable to find records to be deleted!");
            }
            DbContext.Households.Remove(household);
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        //[Route("GetHouseholdMembers/{id:int}")]
        public IHttpActionResult GetHouseholdMembers(int id)
        {
            var userId = User.Identity.GetUserId();
            var householdJoinedUser = DbContext.Households.Where(p => p.Id == id && p.JoinedUsers.Any(m => m.Id == userId))
                .SelectMany(p => p.JoinedUsers)
                .ProjectTo<UsersView>().ToList();

            if (householdJoinedUser.Count() == 0)
            {
                return BadRequest("Unable to find any joined users");
            }
            return Ok(householdJoinedUser);
        }

        [HttpPost]
        //[Route("MemberLeave/{id:int}")]
        public IHttpActionResult MemberLeave(int id)
        {
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(m => m.Id == id);
            if (household == null)
            {
                return BadRequest("Unable to find a valid household!");
            }

            var householdJoinedUser = DbContext.Households.Where(p => p.Id == id && p.JoinedUsers.Any(m => m.Id == userId))
               .Select(p => p.JoinedUsers.FirstOrDefault(m => m.Id == userId)).FirstOrDefault();
            if (householdJoinedUser == null)
            {
                return NotFound();
            }
            if (householdJoinedUser.Email == household.Creator.Email)
            {
                return BadRequest("The owner of a household should not be able to leave the household!");
            }
            household.JoinedUsers.Remove(householdJoinedUser);
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult BankAccountSummary(int id)
        {
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(m => m.Id == id && m.JoinedUsers.Any(p => p.Id == userId));
            if (household == null)
            {
                return BadRequest("Unable to find a valid household!");
            }

            var bankAccounts = DbContext.BankAccounts.Where(m => m.HouseholdId == id && m.Household.JoinedUsers.Any(p => p.Id == userId))
                .Select(m => new HouseholdBankAccountDetailView
                {
                    Id = m.Id,
                    Name = m.Name,
                    Balance = m.Balance,
                    Categorys = m.Transactions.Where(k => k.IfVoid == false).GroupBy(p => p.Category.Name).Select(group => new CategoryGroupView
                    {
                        CategoryName = group.Key,
                        CategoryAmount = group.Sum(g => g.Amount),

                        //Transactions = group.Select(o => new HouseholdBankAccountTransactionDetailView
                        //{
                        //    Id = o.Id,
                        //    Title = o.Title,
                        //    Amount = o.Amount,
                        //    CategoryName = o.Category.Name
                        //}).ToList()
                    }).ToList()
                }).ToList();
            return Ok(bankAccounts);
        }

        [HttpGet]
        public IHttpActionResult TransactionDetails(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(m => m.Id == id && m.Household.JoinedUsers.Any(p => p.Id == userId));
            if (bankAccount == null)
            {
                return BadRequest("Unable to find a valid bankAccount!");
            }
            var transactions = DbContext.Transactions.Where(m => m.BankAccountId == id && m.BankAccount.Household.JoinedUsers.Any(p => p.Id == userId)).Include(k => k.Category)
                .Select(m => new HouseholdBankAccountTransactionDetailView
                {
                    Id = m.Id,
                    Title = m.Title,
                    Amount = m.Amount,
                    CategoryName = m.Category.Name,
                }).ToList();
            return Ok(transactions);
        }

       
    }
}
