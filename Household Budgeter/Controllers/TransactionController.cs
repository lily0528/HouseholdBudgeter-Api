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
    //[RoutePrefix("api/transaction")]
    public class TransactionController : ApiController
    {
        private ApplicationDbContext DbContext;
        public TransactionController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpPost]
        //[Route("create")]
        public IHttpActionResult Create(TransactionBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == formData.BankAccountId && (p.Household.JoinedUsers.Any(j => j.Id == userId) || p.Household.CreatorId == userId));
            if (bankAccount == null)
            {
                return BadRequest("It is invalid bank account!");
            }
            var transaction = Mapper.Map<Transaction>(formData);
            transaction.Created = DateTime.Now;
            transaction.CreatorId = userId;
            transaction.IfVoid = false;
            bankAccount.Balance = bankAccount.Balance + formData.Amount;
            bankAccount.Updated = DateTime.Now;
            DbContext.Transactions.Add(transaction);
            DbContext.SaveChanges();
            var model = Mapper.Map<TransactionView>(transaction);
            return Ok(model);
        }

        [HttpPut]
        //[Route("Edit/{id:int}")]
        public IHttpActionResult Edit(int id, TransactionBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id && p.IfVoid == false);
            if (transaction == null)
            {
                return BadRequest();
            }
            var household = DbContext.Households.FirstOrDefault(p => p.BankAccounts.Any(m => m.Id == formData.BankAccountId) && (p.JoinedUsers.Any(j => j.Id == userId) || p.CreatorId == userId));
            if (household == null)
            {
                return BadRequest("Unable find valid data!");
            }

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == transaction.BankAccountId);

            if (transaction.BankAccountId == formData.BankAccountId && transaction.Amount != formData.Amount)
            {
                bankAccount.Balance = bankAccount.Balance - transaction.Amount + formData.Amount;
                bankAccount.Updated = DateTime.Now;

            }
            else if (transaction.BankAccountId != formData.BankAccountId)
            {
                var bankAccountFormData = DbContext.BankAccounts.FirstOrDefault(p => p.Id == formData.BankAccountId);
                if (bankAccountFormData == null)
                {
                    return NotFound();
                }
                bankAccount.Balance = bankAccount.Balance - transaction.Amount;
                bankAccountFormData.Balance = bankAccountFormData.Balance + formData.Amount;
                bankAccount.Updated = DateTime.Now;
            }
            Mapper.Map(formData, transaction);
            transaction.Amount = formData.Amount;
            DbContext.SaveChanges();
            var model = Mapper.Map<TransactionView>(transaction);
            return Ok(model);
        }

        [HttpDelete]
        //[Route("Delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id && (p.CreatorId == userId || p.BankAccount.Household.CreatorId == userId));
            if (transaction == null)
            {
                return BadRequest("Unable find valid data!");
            }
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == transaction.BankAccountId);
            if (transaction.IfVoid == false)
            {
                bankAccount.Balance = bankAccount.Balance - transaction.Amount;
            }
            DbContext.Transactions.Remove(transaction);
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpPut]
        //[Route("VoidTransaction/{id:int}")]
        public IHttpActionResult VoidTransaction(int id)
        {
            var userId = User.Identity.GetUserId();

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id && p.IfVoid == false && (p.CreatorId == userId || p.BankAccount.Household.CreatorId == userId));
            if (transaction == null)
            {
                return NotFound();
            }
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == transaction.BankAccountId);
            bankAccount.Balance = bankAccount.Balance - transaction.Amount;
            transaction.IfVoid = true;
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        //[Route("GetTransactions/{id:int}")]
        public IHttpActionResult GetTransactions(int id)
        {
            var userId = User.Identity.GetUserId();
            var transaction = DbContext.Transactions.Where(p => p.BankAccountId == id && p.BankAccount.Household.JoinedUsers.Any(m => m.Id == userId))
                .ProjectTo<TransactionView>()
                .ToList();
            return Ok(transaction);
        }
    }
}
