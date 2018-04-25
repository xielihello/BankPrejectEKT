using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VocationalProject.Controllers
{
    public class MallController : Controller
    {
        //
        // GET: /Mall/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyPage404()
        {
            return View();
        }

    }
}
