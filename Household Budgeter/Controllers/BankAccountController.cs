using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    [RoutePrefix("api/BankAccount")]
    public class BankAccountController : ApiController
    {
        private ApplicationDbContext DbContext;

        public BankAccountController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Create(BankAccountBindingModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(p => p.Id == formdata.HouseholdId && p.CreatorId == userId);
            if (household == null)
            {
                return BadRequest("It is invalid household Creator or household!");
            }
            var bankAccount = Mapper.Map<BankAccount>(formdata);
            bankAccount.Balance = 0;
            bankAccount.Created = DateTime.Now;
            DbContext.BankAccounts.Add(bankAccount);
            DbContext.SaveChanges();
            var model = Mapper.Map<BankAccountView>(bankAccount);
            return Ok(model);
        }

        [HttpPut]
        [Route("Edit/{id:int}")]
        public IHttpActionResult Edit(int id, BankAccountBindingModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(p => p.Id == formdata.HouseholdId && p.CreatorId == userId);
            if (household == null)
            {
                return BadRequest("It is invalid household Creator or household!");
            }
            var bankAccountItem = DbContext.BankAccounts.Find(id);
            if (bankAccountItem == null)
            {
                return NotFound();
            }
            Mapper.Map(formdata, bankAccountItem);
            bankAccountItem.Updated = DateTime.Now;
            DbContext.SaveChanges();
            var model = Mapper.Map<BankAccountView>(bankAccountItem);
            return Ok(model);
        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccountItem = DbContext.BankAccounts.Find(id);
            if (bankAccountItem == null)
            {
                return NotFound();
            }

            var household = DbContext.Households.FirstOrDefault(p => p.Id == bankAccountItem.HouseholdId && p.CreatorId == userId);
            if (household == null)
            {
                return BadRequest("It is invalid household Creator or household!");
            }
            DbContext.BankAccounts.Remove(bankAccountItem);
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("GetBankAccounts/{id:int}")]
        public IHttpActionResult GetBankAccounts(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.Households
                .Where(p => p.Id == id && (p.JoinedUsers.Any(j => j.Id == userId) || p.CreatorId == userId))
                .SelectMany(m => m.BankAccounts)
                .ProjectTo<BankAccountView>().ToList();
            return Ok(bankAccount);
        }
        [HttpPost]
        [Route("CalculateBalance/{id:int}")]
        public IHttpActionResult CalculateBalance(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);
            bankAccount.Balance = DbContext.Transactions.Where(p => p.BankAccountId == id && p.IfVoid == false).ToList().Sum(m => m.Amount);
            DbContext.SaveChanges();
            return Ok();
        }
    }
}
