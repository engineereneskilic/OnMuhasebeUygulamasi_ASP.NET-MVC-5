using OnMuhasebeUygulamasi.Models;
using OnMuhasebeUygulamasi.MultipleModelView;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OnMuhasebeUygulamasi.Controllers
{
    public class AccountController : Controller
    {
        private PreliminaryAccountingEntities db = new PreliminaryAccountingEntities();

        string[] StandartUsers = Roles.GetUsersInRole("StandardUser");

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid) // tüm propertyleri set deilmiş mi modelin
            {
                MembershipCreateStatus status;
                Membership.CreateUser(model.UserName, model.Password, model.Email, "soru", "cevap", true, out status);
                FormsAuthentication.SetAuthCookie(model.UserName, false); // false kalıcı cookşe oluşturma
                if (status == MembershipCreateStatus.Success)
                {
                   


                    aspnet_Users updateUser = db.aspnet_Users.Where(get => get.UserName == model.UserName).FirstOrDefault();
                 updateUser.RoleID = 2;

                    db.Entry(updateUser).State = EntityState.Modified;
                 db.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(status));
                }
            }


            return View(model);
        }
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                     

                        return Redirect(returnUrl);
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
                }

            }

            return View(model);
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult ChangePassword()
        {
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            return View();
        }

        [Authorize(Roles = "Admin,StandardUser")]

        // [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool changePaswordSuccessed;

                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true);
                    changePaswordSuccessed = currentUser.ChangePassword(model.OldPassword, model.NewPassword);

                }
                catch
                {
                    changePaswordSuccessed = false;
                }
                if (changePaswordSuccessed)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Başarısız!");
                }

            }
            return View(model);
        }
        private static string ErrorCodeToString(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    return "Aynı mail zaten var!";
                case MembershipCreateStatus.DuplicateUserName:
                    return "Aynı kullanıcı adı zaten var!";
                case MembershipCreateStatus.InvalidAnswer:
                    return "Güvenlik sorusu cevabı geçerli değil!";
                case MembershipCreateStatus.InvalidEmail:
                    return "Geçersiz elektronik posta adresi!";
                case MembershipCreateStatus.InvalidPassword:
                    return "Geçersiz şifre!";
                case MembershipCreateStatus.InvalidQuestion:
                    return "Güvenlik sorusu geçerli değil!";
                case MembershipCreateStatus.InvalidUserName:
                    return "Geçersiz kullanıcı adı!";
                case MembershipCreateStatus.ProviderError:
                    return "Sistem yöneticisine başvurun!";
                case MembershipCreateStatus.UserRejected:
                    return "Kullanıcı oluşturma işlemi iptal edildi! Eğer hata devam ederse yöneticiye başvurun.";
                default:
                    return "Bilinmeyen bir hata oluştu!";
            }
        }
        List<UserDetailsModel> udmList = new List<UserDetailsModel>();

        public ActionResult UserDetails()
        {
            if (User.Identity.IsAuthenticated && db.aspnet_Users.Where(au => au.UserName == User.Identity.Name) != null) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            udmListFull();


            return View(udmList);
        }
        public void udmListFull()
        {
            var Users = db.aspnet_Membership   // your starting point - table in the "from" statement
.Join(db.aspnet_Users, // the source table of the inner join
  m => m.UserId,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
  u => u.UserId,   // Select the foreign key (the second part of the "on" clause)
  (m, u) => new { M = m, U = u }).ToList(); // selection 

            




            foreach (var item in Users)
            {
              
                UserDetailsModel udm = new UserDetailsModel();
                udm.UDid = item.M.UserId;
                udm.Roleid = item.U.RoleID;
                udm.RoleName = db.Roles.Where(r => r.RoleID == item.U.RoleID).FirstOrDefault().RoleName;
                udm.UserName = item.U.UserName;
                udm.Password = item.M.Password;
                udm.Email = item.M.Email;
                udm.RoleList = db.Roles.ToList();
                udmList.Add(udm);
            }

        }


        public ActionResult Edit(Guid? id)
        {
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();

            if (id == null)
            {
                return Redirect("/Home/Index");
            }

            udmListFull();

            UserDetailsModel fudm = udmList.Where(d => d.UDid == id).FirstOrDefault();
            


            return View(fudm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserDetailsModel udms)
        {
            aspnet_Membership UpdateAm = db.aspnet_Membership.Where(ar => ar.UserId == udms.UDid).FirstOrDefault();
            UpdateAm.Email = udms.Email;
            UpdateAm.Password = udms.Password;

            db.Entry(UpdateAm).State = System.Data.Entity.EntityState.Modified;




            aspnet_Users UpdateAu = db.aspnet_Users.Where(u => u.UserId == udms.UDid).FirstOrDefault();
            UpdateAu.UserName = udms.UserName;
            UpdateAu.RoleID = udms.Roleid;

            db.Entry(UpdateAu).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return Redirect("/Account/UserDetails");
        }




        public ActionResult Delete(Guid? id)
        {
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            if (id == null)
            {
                return Redirect("/Home/Index");
            }

            udmListFull();

            UserDetailsModel fudm = udmList.Where(d => d.UDid == id).FirstOrDefault();

            return View(fudm);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            aspnet_Membership UpdateAm = db.aspnet_Membership.Where(ar => ar.UserId == id).FirstOrDefault();
            db.aspnet_Membership.Remove(UpdateAm);

            aspnet_Users UpdateAu = db.aspnet_Users.Where(u => u.UserId == id).FirstOrDefault();

            db.aspnet_Users.Remove(UpdateAu);

            db.SaveChanges();

            // şuanki kullanıcı kendini silmek isterse sildiği an oturumu kapansın
            if(User.Identity.Name== UpdateAu.UserName)
            {
                return Redirect("/Account/LogOut");
            }
            else
            {
                return Redirect("/Account/UserDetails");
            }

            
        }
 
    }
}
