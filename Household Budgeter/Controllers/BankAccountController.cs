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
    //[RoutePrefix("api/BankAccount")]
    public class BankAccountController : ApiController
    {
        private ApplicationDbContext DbContext;

        public BankAccountController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpPost]
        //[Route("Create")]
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
                ModelState.AddModelError("", "It is invalid household Creator or household!");
                return BadRequest(ModelState);
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
        //[Route("Edit/{id:int}")]
        public IHttpActionResult Edit(int id, EditBankAccountBindingModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();

            var bankAccountItem = DbContext.BankAccounts.FirstOrDefault(p => p.Id ==id && p.Household.CreatorId == userId);
            if (bankAccountItem == null)
            {
                ModelState.AddModelError("", "It is invalid household Creator or household!");
                return BadRequest(ModelState);
            }
            Mapper.Map(formdata, bankAccountItem);
            bankAccountItem.Updated = DateTime.Now;
            DbContext.SaveChanges();
            var model = Mapper.Map<BankAccountView>(bankAccountItem);
            return Ok(model);
        }

        [HttpDelete]
        //[Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccountItem = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id && p.Household.CreatorId == userId);
            if (bankAccountItem == null)
            {
                return NotFound();
            }
            DbContext.BankAccounts.Remove(bankAccountItem);
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        //[Route("GetBankAccounts/{id:int}")]
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
        //[Route("CalculateBalance/{id:int}")]
        public IHttpActionResult CalculateBalance(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id && p.Household.CreatorId == userId);
            if (bankAccount == null)
            {
                return NotFound();
            }
            bankAccount.Balance = DbContext.Transactions.Where(p => p.BankAccountId == id && p.IfVoid == false && p.BankAccount.Household.CreatorId == userId).ToList().Sum(m => (decimal?)m.Amount?? 0);
            DbContext.SaveChanges();
            return Ok();
        }
    }
}
