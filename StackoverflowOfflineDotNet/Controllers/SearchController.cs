using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StackoverflowOfflineDotNet.Service;

namespace StackoverflowOfflineDotNet.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        [HttpGet]
        public ActionResult Search(string id)
        {
            return Json(SearchService.Search(id), JsonRequestBehavior.AllowGet);
        }
    }
}
