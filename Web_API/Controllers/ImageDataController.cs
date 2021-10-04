using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Web_API.Models;

namespace Web_API.Controllers
{
    [Authorize]
    public class ImageDataController : ApiController
    {
        private ImageDataContext db = new ImageDataContext();

        // GET: api/ImageData
        //get for current user
        public IQueryable<ImageDataModel> GetImageDataModels()
        {
            string username = User.Identity.Name;
            return db.ImageDataModels.Where(item => item.Username == username);
        }

        [ResponseType(typeof(int))]
        [Route("api/ImageData/CurrentUser")]
        public IHttpActionResult GetNumItemsCurrentUser()
        {
            Debug.WriteLine("Current user");
            string currentUser = User.Identity.Name;

            return Ok(db.ImageDataModels.Count(item => item.Username == currentUser));

        }

        /*[ResponseType(typeof(int))]
        [HttpGet]
        public IHttpActionResult GetNumberOfItems_CurrentUser(string name)
        {
            if (name != User.Identity.Name) return BadRequest("Invalid operation...");

            return Ok(db.ImageDataModels.Count(item => item.Username == User.Identity.Name));
        }*/

        // GET: api/ImageData/5
        [ResponseType(typeof(ImageDataModel))]
        public IHttpActionResult GetImageDataModel(int id)
        {
            ImageDataModel imageDataModel = db.ImageDataModels.Find(id);
            if (imageDataModel == null)
            {
                return NotFound();
            }

            return Ok(imageDataModel);
        }


        // PUT: api/ImageData/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutImageDataModel(int id, ImageDataModel imageDataModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != imageDataModel.Id)
            {
                return BadRequest();
            }

            db.Entry(imageDataModel).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageDataModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ImageData
        [ResponseType(typeof(ImageDataModel))]
        public IHttpActionResult PostImageDataModel(ImageDataModel imageDataModel)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ImageDataModels.Add(imageDataModel);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = imageDataModel.Id }, imageDataModel);
        }

        // DELETE: api/ImageData/5
        [ResponseType(typeof(ImageDataModel))]
        public IHttpActionResult DeleteImageDataModel(int id)
        {
            ImageDataModel imageDataModel = db.ImageDataModels.Find(id);
            if (imageDataModel == null)
            {
                return NotFound();
            }

            db.ImageDataModels.Remove(imageDataModel);
            db.SaveChanges();

            return Ok(imageDataModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ImageDataModelExists(int id)
        {
            return db.ImageDataModels.Count(e => e.Id == id) > 0;
        }
    }
}