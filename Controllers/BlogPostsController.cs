using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wonderly_Blog.Models;
using Wonderly_Blog.Models.CodeFirst;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace Wonderly_Blog.Controllers
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: BlogPosts
        public ActionResult Index(int? page, string query)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.Query = query;
            var qposts = db.Posts.AsQueryable();

            if(!string.IsNullOrWhiteSpace(query))
            {
                qposts = qposts.Where(p => p.Title.Contains(query) || p.Body.Contains(query) ||
                p.Comments.Any(c => c.Body.Contains(query) || c.Author.DisplayName.Contains(query)));
            }

            var posts = qposts.OrderByDescending(p=>p.Created).ToPagedList(pageNumber, pageSize);
            return View(posts);
        }

        // Directory controller
        [Authorize (Roles="Admin, Moderator")]
        public ActionResult Directory()
        {
            return View(db.Posts.ToList());
        }

        [Authorize]
        // GET: BlogPosts/Details/5
        public ActionResult Details(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BlogPost blogPost = db.Posts.FirstOrDefault(p=> p.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }

            return View(blogPost); //Previously had post or blogPost inside View();
        }

        // GET: BlogPosts/Create
        [Authorize (Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Create([Bind(Include = "Id,Title,Slug,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                var id = blogPost.Id;

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/img/blog"), fileName));
                    blogPost.MediaURL = "~/img/blog/" + fileName;
                }

                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(blogPost);
                }

                if( db.Posts.Any(p=>p.Id == id))
                {
                    ModelState.AddModelError("Title", "The title must be unique.");
                    return View(blogPost);
                }
                blogPost.Slug = Slug;
                blogPost.Created = DateTimeOffset.Now;
                //blogPost.Updated = DateTimeOffset.Now;
                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");


            }

            return View(blogPost);
        }

        [Authorize (Roles = "Admin, Moderator")]
        // GET: BlogPosts/Edit/5
        public ActionResult Edit(string slug)
        {
            
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == slug);

            if (blogPost == null)
            {
                return HttpNotFound();
            }
            
          
           
            //return View(blogPost);


            /*
            BlogPost post = db.Posts.FirstOrDefault(p => p.Slug == slug);
            if (post == null)
            {
                return HttpNotFound();
            }
            */
            

            return View(blogPost);

        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {


                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/img/blog"), fileName));
                    blogPost.MediaURL = "~/img/blog/" + fileName;
                }


                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                var PostId = blogPost.Id;

                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(blogPost);
                }
                
                /* Start Ele's code */
                var SlugAlreadyExists = db.Posts.Where(p => p.Id == PostId && p.Slug == Slug).Select(p => p.Slug);

                if (!SlugAlreadyExists.Any())
                {
                    if (db.Posts.Any(p => p.Slug == Slug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique.");
                        return View(blogPost);
                    }
                }

                blogPost.Slug = Slug;
                blogPost.Updated = System.DateTimeOffset.Now;
                db.Posts.Attach(blogPost);
                db.Entry(blogPost).Property("Title").IsModified = true;
                db.Entry(blogPost).Property("Body").IsModified = true;
                //db.Entry(blogPost).Property("MediaURL").IsModified = false;

                if ( blogPost.MediaURL != null )
                {
                   db.Entry(blogPost).Property("MediaURL").IsModified = true;
                }

                db.Entry(blogPost).Property("Slug").IsModified = true;
                db.Entry(blogPost).Property("Updated").IsModified = true;
                //db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Directory");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize (Roles = "Admin, Moderator")]
        public ActionResult Delete(string slug)
        {

            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == slug);

            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string slug)
        {
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == slug);
            db.Posts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Directory");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        [Authorize]
        public ActionResult Comment()
        {
            Contact c = new Contact();
            ViewBag.Name = "Ria";
            ViewBag.Message = "Your contact page.";

            return View(c);
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comment([Bind(Include = "Id,PostID,Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.AuthorID = User.Identity.GetUserId();
                comment.Created = DateTimeOffset.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
            }

            var post = db.Posts.Find(comment.PostID);
            return RedirectToAction("Details", new { slug = post.Slug });
        }

        [Authorize(Roles = "Admin, Moderator")]
        // GET: BlogPosts/Edit/5
        public ActionResult EditComment(int commentID)
        {


            Comment comment = db.Comments.FirstOrDefault(p => p.Id == commentID);

            if (comment == null)
            {
                return HttpNotFound();
            }



            //return View(blogPost);


            /*
            BlogPost post = db.Posts.FirstOrDefault(p => p.Slug == slug);
            if (post == null)
            {
                return HttpNotFound();
            }
            */


            return View(comment);

        }

        // POST: BlogPosts/EditComment/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment([Bind(Include = "Id,Body,Updated,PostID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                //var body = comment.Body;

                if (String.IsNullOrWhiteSpace(comment.Body))
                {
                    ModelState.AddModelError("Body", "A comment body is required.");
                    return View(comment);
                }


                
                comment.Updated = System.DateTimeOffset.Now;
                db.Comments.Attach(comment);

                //db.Entry(blogPost).State = EntityState.Modified;
                db.Entry(comment).Property("Body").IsModified = true;
                db.Entry(comment).Property("Updated").IsModified = true;
                db.SaveChanges();
                var post = db.Posts.Find(comment.PostID);
                return RedirectToAction("Details", new { slug = post.Slug });
            }
            return View(); //formerly had comment inside parentheses
        }

        // GET: BlogPosts/DeleteComment/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult DeleteComment(int commentID)
        {

            Comment comment = db.Comments.FirstOrDefault(p => p.Id == commentID);

            if (comment == null)
            {
                return HttpNotFound();
            }

            return View(comment);
        }

        // POST: BlogPosts/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCommentConfirmed(int commentID)
        {
            Comment comment = db.Comments.FirstOrDefault(p => p.Id == commentID);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Directory");
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult DeleteImage(string slug)
        {

            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == slug);

            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("DeleteImage")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteImageConfirmed(string slug)
        {
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.Slug == slug);
            blogPost.MediaURL = null;
            db.SaveChanges();
            return RedirectToAction("Index");
        }




    }
}
