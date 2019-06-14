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
    //[RoutePrefix("api/Category")]
    public class CategoryController : ApiController
    {
        private ApplicationDbContext DbContext;

        public CategoryController()
        {
            DbContext = new ApplicationDbContext();
        }
   
        [HttpPost]
        //[Route("Create")]
        public IHttpActionResult Create(CategoryBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.FirstOrDefault(p => p.Id == model.HouseholdId && p.CreatorId == userId);
            if (household == null)
            {
                return NotFound();
            }
            var category = Mapper.Map<Category>(model);
                category.Created = DateTime.Now;

            DbContext.Categories.Add(category);
            DbContext.SaveChanges();
            var categoryModel = Mapper.Map<CategoryView>(category);
            return Ok(categoryModel);
        }


        [HttpGet]
        public IHttpActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var category = DbContext.Categories.Where(p => p.Id == id && p.Household.CreatorId == userId)
                .Select(p => new ViewCategoryView
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsOwner = p.Household.CreatorId == userId,
                    Description = p.Description,
                    HouseholdId = p.HouseholdId
                }).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }


        [HttpPut]
        //[Route("{id}")]
        public IHttpActionResult Edit(int id, CategoryBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var category = DbContext.Categories.FirstOrDefault(p => p.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            var household = DbContext.Households.FirstOrDefault(p => p.Id == category.HouseholdId && p.CreatorId == userId);

            if (household == null)
            {
                return BadRequest("It is invalid household Creator or household!");
            }

            Mapper.Map(model, category);
            category.Updated = DateTime.Now;
            DbContext.SaveChanges();
            var categoryModel = Mapper.Map<CategoryView>(category);
            return Ok(categoryModel);
        }

        [HttpDelete]
        //[Route("delete/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var category = DbContext.Categories.FirstOrDefault(p => p.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            var household = DbContext.Households.FirstOrDefault(p => p.Id == category.HouseholdId && p.CreatorId == userId);
            if (household == null)
            {
                return BadRequest("It is invalid household Creator or household!");
            }

            DbContext.Categories.Remove(category);
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult GetCategoriesSelectList(int id)
        {
            var userId = User.Identity.GetUserId();
            var result = DbContext.Categories.Where(p => p.HouseholdId == id &&  (p.Household.CreatorId == userId || p.Household.JoinedUsers.Any(t => t.Id == userId)))
              .Select(p => new ViewCategoryView
              {
                  Id = p.Id,
                  Name = p.Name
              }).ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetCategoriesSelectListByTransaction(int id)
        {
            var userId = User.Identity.GetUserId();
            var household = DbContext.Households.Where(p => p.Categories.Any(j => j.Transactions.Any(m => m.Id == id))).FirstOrDefault();
            if (household == null)
            {
                return NotFound();
            }
            var result = DbContext.Categories.Where(p => p.HouseholdId == household.Id && (p.Household.CreatorId == userId || p.Household.JoinedUsers.Any(t => t.Id == userId)))
              .Select(p => new ViewCategoryView
              {
                  Id = p.Id,
                  Name = p.Name
              }).ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult ViewCategory()
        {
            var userId = User.Identity.GetUserId();
            var result = DbContext.Categories.Where(p => p.Household.CreatorId == userId || p.Household.JoinedUsers.Any(t => t.Id == userId)).Include(j => j.Household)
                .Select(p => new ViewCategoryView
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsOwner = p.Household.CreatorId == userId,
                    Description = p.Description,
                    //HouseholdId = p.HouseholdId,
                    HouseholdName = p.Household.Name 
                }).ToList();

            return Ok(result);
        }

    }
}
