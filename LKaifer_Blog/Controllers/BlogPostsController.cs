using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LKaifer_Blog.Models;
using LKaifer_Blog.Utilities;
using PagedList;
using PagedList.Mvc;

namespace LKaifer_Blog.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller

    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
            
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);
            int pageSize = 4;//display three blog posts at a time on this page
            int pageNumber = page ?? 1;

            ViewBag.CarouselPosts = db.BlogPosts.OrderByDescending(b => b.Created).Take(3).ToList();
            
            return View(blogList.ToPagedList(pageNumber, pageSize));
        }

        public IQueryable<BlogPost>IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) || p.Body.Contains(searchStr) || p.Comments.Any(c => c.Body.Contains(searchStr) || c.Author.FirstName.Contains(searchStr) || c.Author.LastName.Contains(searchStr) || c.Author.DisplayName.Contains(searchStr) || c.Author.Email.Contains(searchStr)));

            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);

        }
        [AllowAnonymous]
        // GET: BlogPosts/Details/5
        public ActionResult Details(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IEnumerable<BlogPost> blogPostList = db.BlogPosts;
            var blogPost = blogPostList.FirstOrDefault(b => b.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            ViewBag.PreviousPost = blogPostList.OrderByDescending(p => p.Id).SkipWhile(p => p.Id != blogPost.Id).Skip(1).FirstOrDefault();
            ViewBag.NextPost = blogPostList.SkipWhile(n => n.Id != blogPost.Id).Skip(1).FirstOrDefault();

            return View(blogPost);
        }



        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }


        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {


            if (ModelState.IsValid)
            {


                //This is where we initally create the slug based soley on the incoming Title from the form.
                var Slug = StringUtilities.SlugMaker(blogPost.Title);


                //There is no guarantee that we can use the slug because if it is empty. . . it's bad
                if (string.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                //If the slug is already present in the db. . .its bad
                if (db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The Title must be unique");
                    return View(blogPost);
                }



                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaURL = "~/Uploads/" + fileName;
                }
                //Otherwise the slug is good and we assign it to the Slug property and save it to the DB
                blogPost.Slug = Slug;
                blogPost.Created = DateTimeOffset.Now;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);

        }




        //GET:  BlogPosts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            BlogPost blogpost = db.BlogPosts.Find(id);
            if (blogpost == null)
            {
                return HttpNotFound();
            }
            return View(blogpost);

        }



        // POST BlogPosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit([Bind(Include = "Id,Title,Abstract,Slug,Body,MediaUrl,Published,Created,Updated")]BlogPost blogpost, HttpPostedFileBase image)

        {
            if (ModelState.IsValid)
            {
                //This is where we recreate the slug based soley on the incoming Title from the form.
                var newslug = StringUtilities.SlugMaker(blogpost.Title);

                // IF AND ONLY IF THE SLUG (WHICH IS BUILT FROM THE TITLE) HAS CHANGED DO WE NEED TO REP
                if (newslug != blogpost.Slug)
                {

                    //There is no guarantee that we can use the slug because if it is empty. . . it's bad
                    if (string.IsNullOrWhiteSpace(newslug))
                    {
                        ModelState.AddModelError("Title", "Invalid title");
                        return View(blogpost);
                    }
                    //If the slug is already present in the db. . .its bad
                    if (db.BlogPosts.Any(p => p.Slug == newslug))
                    {
                        ModelState.AddModelError("Title", "The Title must be unique");
                        return View(blogpost);
                    }
                    //Otherwise the slug is good and we assign it to the Slug property and save it to the DB
                    blogpost.Slug = newslug;
                }
                if (image != null)
                {
                    if (ImageUploadValidator.IsWebFriendlyImage(image))
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                        blogpost.MediaURL = "~/Uploads/" + fileName;
                    }
                }
                blogpost.Updated = DateTimeOffset.Now;
                db.Entry(blogpost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(blogpost);

        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }








        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
