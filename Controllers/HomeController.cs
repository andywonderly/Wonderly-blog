using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wonderly_Blog.Models;

namespace Wonderly_Blog.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Abc()
        {
            int x = 7;
            x += 5;
            ViewBag.Pqr = "Test test I am the best.";
            //ViewBag.Pqr = x;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            Contact c = new Contact();
            ViewBag.Name = "Ria";
            ViewBag.Message = "Your contact page.";

            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact([Bind(Include = "Id,Name,Email,Message,Phone,MessageSent")] Contact contact)
        {
            contact.MessageSent = DateTime.Now;
            var svc = new EmailService();
            var msg = new IdentityMessage();
            msg.Subject = "BlogContactForm";
            msg.Body = contact.Message;
            await svc.SendAsync(msg);

            return View(contact);

        }
    }
}