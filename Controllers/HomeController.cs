using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VocationalProject.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index(int Type = 0)
        {
            if (Type == 0)
            {
                if (UserType == "1")
                {
                    Response.Redirect("/Admin/CollegeManagement/College");
                }
                else if (UserType == "2")
                {
                    Response.Redirect("/Admin/T_StudentManage");
                }
                else if (UserType == "3")
                {
                    Response.Redirect("/HB_Competition");
                }
            }
            else
            {
                Session.Abandon();
                Session.Clear();
                Response.Redirect("/login");

            }
            return View();

        }

    }
}
