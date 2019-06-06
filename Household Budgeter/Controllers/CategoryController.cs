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
    //[RoutePrefix("api/Category")]
    public class CategoryController : ApiController
    {
        private ApplicationDbContext DbContext;

        public CategoryController()
        {
            DbContext = new ApplicationDbContext();
        }
        //[HttpGet]
        //public IHttpActionResult Create()
        //{
        //    var userId = User.Identity.GetUserId();
        //    var household = DbContext.Households.Where(p => p.CreatorId == userId).ToList();
        //    var model = new CategoryBindingModel();
        //    //Todo:SELECTLIST
        //    model.Household = new SelectList(household, "Id", "Name");
        //    return Ok(model);
            
        //}
   
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
            //var category = new Category
            //{
            //    Name = model.Name,
            //    Description = model.Description,
            //    Created = DateTime.Now,
            //    HouseholdId = model.HouseholdId
            //};
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
        public IHttpActionResult GetCategoriesSelectList()
        {
            var userId = User.Identity.GetUserId();
            var result = DbContext.Categories.Where(p => p.Household.CreatorId == userId || p.Household.JoinedUsers.Any(t => t.Id == userId))
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
            var result = DbContext.Categories.Where(p => p.Household.CreatorId == userId || p.Household.JoinedUsers.Any(t => t.Id == userId))
                .Select(p => new ViewCategoryView
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsOwner = p.Household.CreatorId == userId,
                    Description = p.Description,
                    HouseholdId = p.HouseholdId
                }).ToList();

            return Ok(result);
        }



        //[HttpGet]
        ////[Route("GetCategory/{id:int}")]
        //public IHttpActionResult GetCategory(int id)
        //{
        //    var userId = User.Identity.GetUserId();
        //    var category = DbContext.Categories.Where(p => p.HouseholdId == id && p.Household.JoinedUsers.Any(m => m.Id == userId))
        //                   .ProjectTo<CategoryView>().ToList();
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(category);
        //}
    }
}
