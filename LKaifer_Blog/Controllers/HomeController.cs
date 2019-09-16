using LKaifer_Blog.Models;
using LKaifer_Blog.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace LKaifer_Blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        //public ActionResult Index(int? page)
        //{
        //    int pageSize = 3;//display three blog posts at a time on this page
        //    int pageNumber = (page ?? 1);
        //    return View();
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var from = $"{model.FromEmail}<{WebConfigurationManager.AppSettings["emailto"]}>";
                    var email = new MailMessage(from,
                        WebConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = model.Subject,
                        Body = $"You have an email from {model.FromName}<br/>{model.Body}",
                        IsBodyHtml = true
                    };
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                    ViewBag.SentConfirmationMessage = "Message has been successfully sent.";
                    return View(model);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return RedirectToAction("Index", "BlogPosts");
            
        }
    }



}

