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
            var category = DbContext.Categories.FirstOrDefault(p => p.Id == formData.CategoryId && p.HouseholdId == bankAccount.HouseholdId);
            if (category == null)
            {

                ModelState.AddModelError("", "Category doesn't exist in this household");
                return BadRequest(ModelState);
            }
            var transaction = Mapper.Map<Transaction>(formData);
            transaction.Created = DateTime.Now;
            transaction.Date = formData.Date;
            transaction.CreatorId = userId;
            transaction.IfVoid = false;
            bankAccount.Balance += formData.Amount;
            bankAccount.Updated = DateTime.Now;
            DbContext.Transactions.Add(transaction);
            DbContext.SaveChanges();
            var model = Mapper.Map<TransactionView>(transaction);
            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id && p.IfVoid == false && (p.CreatorId == userId || p.BankAccount.Household.CreatorId == userId));
            var model =  new EditTransactionBindingModel
                {
                    Title = transaction.Title,
                    Description = transaction.Description,
                    Amount = transaction.Amount,
                    Date = transaction.Date,
                    BankAccountId = transaction.BankAccountId,
                    IsOwner = transaction.BankAccount.Household.CreatorId == userId || transaction.BankAccount.Household.JoinedUsers.Any(m => m.Id == userId),
                    CategoryId = transaction.CategoryId
            };
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpPut]
        public IHttpActionResult Edit(int id, TransactionBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id && p.IfVoid == false && (p.CreatorId == userId || p.BankAccount.Household.CreatorId == userId));
            if (transaction == null)
            {
                return NotFound();
            }

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == transaction.BankAccountId);
            if (bankAccount == null)
            {
                return NotFound();
            }

            var category = DbContext.Categories.FirstOrDefault(p => p.Id == formData.CategoryId && p.HouseholdId == bankAccount.HouseholdId);
            if (category == null)
            {

                ModelState.AddModelError("", "Category doesn't exist in this household");
                return BadRequest(ModelState);
            }

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
                bankAccount.Balance -= transaction.Amount;
                bankAccountFormData.Balance += formData.Amount;
                bankAccount.Updated = DateTime.Now;
            }
            Mapper.Map(formData, transaction);
            transaction.Amount = formData.Amount;
            DbContext.SaveChanges();
            var model = Mapper.Map<TransactionView>(transaction);
            return Ok(model);
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id && (p.CreatorId == userId || p.BankAccount.Household.CreatorId == userId));
            if (transaction == null)
            {
                return NotFound();
            }
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == transaction.BankAccountId);
            if(bankAccount == null)
            {
                return BadRequest("Unable find valid bank account!");
            }
            if (transaction.IfVoid == false)
            {
                bankAccount.Balance -= transaction.Amount;
            }
            DbContext.Transactions.Remove(transaction);
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpPut]
        public IHttpActionResult VoidTransaction(int id)
        {
            var userId = User.Identity.GetUserId();

            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id && p.IfVoid == false && (p.CreatorId == userId || p.BankAccount.Household.CreatorId == userId));
            if (transaction == null)
            {
                return NotFound();
            }
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == transaction.BankAccountId);
            if (bankAccount == null)
            {
                return BadRequest("Unable to find valid bank account!");
            }
            bankAccount.Balance = bankAccount.Balance - transaction.Amount;
            transaction.IfVoid = true;
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult ViewTransactions()
        {
            //var userId = User.Identity.GetUserId();
            //var transactions = DbContext.Transactions.Where(p => p.BankAccount.Household.JoinedUsers.Any(m => m.Id == userId)).Include(k => k.Category).Include(j =>j.BankAccount).ToList();
            //var model = Mapper.Map<List<ViewTransactionView>>(transactions);
            var userId = User.Identity.GetUserId();
            var result = DbContext.Transactions.Where(p =>  p.IfVoid == false && p.BankAccount.Household.JoinedUsers.Any(m => m.Id == userId)).Include(k => k.Category).Include(j => j.BankAccount)
                .Select(p => new ViewTransactionView
                {
                    Id = p.Id,
                    Title = p.Title,
                    IsOwner = p.BankAccount.Household.CreatorId == userId  || p.BankAccount.Household.JoinedUsers.Any(m => m.Id == userId),
                    Description = p.Description,
                    Amount = p.Amount,
                    Date = p.Date,
                    BankAccountName = p.BankAccount.Name,
                    CategoryName = p.Category.Name
                }).ToList();

            return Ok(result);
            
        }

        [HttpGet]
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
