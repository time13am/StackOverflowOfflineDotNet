using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StackoverflowOfflineDotNet.Service;

namespace StackoverflowOfflineDotNet.Controllers
{
    public class QuestionController : Controller
    {
        //
        // GET: /Question/

        public ActionResult Question(int id)
        {
            var posts = QuestionService.process_page(id);
            ViewBag.Title = posts.ElementAt(0).title;
            ViewBag.Posts = posts;
            return View();
        }

    }
}
