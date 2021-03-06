﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TodoList.Data;
using TodoList.Models;

namespace TodoList.Controllers
{
    [RoutePrefix("api/categories")]
    public class CategoriesController : ApiController
    {
        //ouverture de la conexion à la db
        private TodoListDbContext db = new TodoListDbContext();

        //GET: api/categories
        public IQueryable<Category> GetCategories()
        {
            return db.Categories.Where(x => !x.Deleted);
        }

        //GET: api/categories/1
        /// <summary>
        /// Recherche une categorie par son ID
        /// </summary>
        /// <param name="id">l'id en entier</param>
        /// <returns></returns>
        [Route("{id:int}")]
        [ResponseType(typeof(Category))]
        public IHttpActionResult GetCategories(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Recherche des catégories par le contenu du nom
        /// </summary>
        /// <remarks>Renvoie les catégories dont le nom contient la valeur name (CI).</remarks>
        /// <param name="name">Nom a rechercher</param>
        /// <returns></returns>
        //GET: api/categories/name
        [Route("{name}")]
        [ResponseType(typeof(Category))]
        public IQueryable<Category> GetCategories(string name)
        {
            return db.Categories.Where(x => !x.Deleted && x.Name.Contains(name));
        }



        /// <summary>
        /// Ajoute une categorie dans la base. Le nom est obligatoire
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        //POST: api/categories
        [ResponseType(typeof(Category))]
        public IHttpActionResult PostCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Categories.Add(category);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = category.ID }, category);
        }

        
        //PUT: api/categories/1
        [Route("{id}")]
        [ResponseType(typeof(Category))]
        public IHttpActionResult PutCategory(int id, Category category)
        {
            if (id != category.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(db.Categories.Find(id).Deleted)
                return BadRequest();

            db.Entry(category).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        //DELETE: api/categories/1
        [Route("{id}")]
        [ResponseType(typeof(Category))]
        public IHttpActionResult DeleteCategory(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
                return NotFound();

            //db.Categories.Remove(category);
            category.Deleted = true;
            category.DeletedAt = DateTime.Now;
            db.Entry(category).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Ok(category);
        }


        //réécriture de la methode dispose pour libérer en mémoire le DbContext 
        //et donc la connexion
        //methode dispose appelée lorsque que IIS n'utilise plus le controller
        protected override void Dispose(bool disposing)
        {
            db.Dispose();//libère le db context
            base.Dispose(disposing);
        }
    }
}
