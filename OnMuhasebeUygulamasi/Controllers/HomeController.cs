using OnMuhasebeUygulamasi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OnMuhasebeUygulamasi.Controllers
{
    public class HomeController : Controller
    {
        private PreliminaryAccountingEntities db = new PreliminaryAccountingEntities();

        // GET: Home
        public ActionResult Index()
        {
            /*
            Roles.CreateRole("Admin");
            Roles.CreateRole("StandardUser");
            */

            
            if(User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();

            if (!User.Identity.IsAuthenticated) return RedirectToAction("LogOn", "Account"); else return View();

            
        }



        /*
        public  ActionResult CreateRole()
        {
            try
            {
                Roles.CreateRole("User");

                return RedirectToAction("Index","Home");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }
            
            return View();
        }
        */
    }
}