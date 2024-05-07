using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnMuhasebeUygulamasi.Models;
using System.Net;
using System.Data.Entity;
using OnMuhasebeUygulamasi.MultipleModelView;

namespace OnMuhasebeUygulamasi.Controllers
{
    public class CurrentCardsController : Controller
    {
        // GET: CurrentCards

        private PreliminaryAccountingEntities db = new PreliminaryAccountingEntities();

        public ActionResult Index(int? id)
        {
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            var fullcclist =
                from cc in db.CurrentCards
                select cc;
            
            // ilk değerli null yap çünkü en baştan hesaplanacak
            (from cc in db.CurrentCards

             select cc).ToList().ForEach(m => m.BalanceCredit = 0);



            (from cc in db.CurrentCards

             select cc).ToList().ForEach(m => m.BalanceDebt = 0);

            var cd = db.CurrentCardDetails.Where(cdd => cdd.CurrentCode == id).FirstOrDefault();

            if (cd != null)
            {


                // Carikart alacakları toplayıp güncelle
                (from am in db.AccountMovements
                 join cc in db.CurrentCards on
                  am.CurrentCode
                 equals
                  cc.CurrentCode
                 select new { am, cc }).Where(m => m.am.Credit != null).ToList().ForEach(m => m.cc.BalanceCredit += m.am.Credit);



                //Cari kart borçları toplayıp güncelle
                (from am in db.AccountMovements
                 join cc in db.CurrentCards on
                 am.CurrentCode
                 equals
                 cc.CurrentCode
                 select new { am, cc }).Where(m => m.am.Debt != null).ToList().ForEach(m => m.cc.BalanceDebt += m.am.Debt);

            }
            db.SaveChanges();



            if (User.Identity.Name == "") return RedirectToAction("LogOn", "Account"); else
            {
                if(id == null)
                {
                    CurrentCardswithAC cwd =  new CurrentCardswithAC()
                    {
                        CurrentCardList = fullcclist.ToList()
                    };

                    return View(cwd);
                }
                else
                {
                    var amList =
                from am in db.AccountMovements where am.CurrentCode == id
                select am;
                    CurrentCardswithAC cwd = new CurrentCardswithAC()
                    {
                        CurrentCardList = fullcclist.ToList(),
                        AccountMovementList = amList.ToList() 
                    };

                    return View(cwd);
                }
            }

        }

        // GET: Products/Create
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            return View();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CurrentCardDetail cd)
        {
            if (ModelState.IsValid)
            {
                db.CurrentCardDetails.Add(cd);
                db.SaveChanges();

                db.CurrentCards.Add(cd.CurrentCard);
                db.SaveChanges();

                return Redirect("/CurrentCards/Index");
            }


            return View(cd);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            if (id == null)
            {
                return Redirect("/CurrentCards/Index");
            }
            CurrentCardDetail ccdt = db.CurrentCardDetails.Find(id);
            if (ccdt == null)
            {
                return HttpNotFound();
            }
            return View(ccdt);
        }
        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            if (id == null)
            {
                return Redirect("/CurrentCards/Index");
            }
           
            CurrentCardDetail cc = db.CurrentCardDetails.Find(id);
            if (cc == null)
            {
                return Redirect("/CurrentCards/Index");
            }
            return View(cc);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CurrentCardDetail cd)
        {
          //  CurrentCard cc = db.CurrentCards.Find(cd.CurrentCode);

                
            if (ModelState.IsValid)
            {
                db.Entry(cd).State = EntityState.Modified;

                CurrentCard updateCurrentCard = (from upc in db.CurrentCards where upc.CurrentCode == cd.CurrentCode select upc).FirstOrDefault();
                updateCurrentCard.CurrentName = cd.CurrentCard.CurrentName;
                updateCurrentCard.CurrentType = cd.CurrentCard.CurrentType;

                db.Entry(updateCurrentCard).State = EntityState.Modified;

                // tüm bu cari karta ait bilgiler güncellensin
                (from sm in db.StockMovements where sm.CurrentCode == cd.CurrentCode select sm).ToList().ForEach(up => up.CurrentName = cd.CurrentCard.CurrentName);

                (from bb in db.Bills where bb.CurrrentCode == cd.CurrentCode select bb).ToList().ForEach(up => up.CurrentName = cd.CurrentCard.CurrentName);



                // Bill updateBill = db.Bills()

                db.SaveChanges();
                
                return RedirectToAction("Index");
            }
           
            return View(cd);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {

            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            if (id == null)
            {
                return Redirect("/CurrentCards/Index");
            }
            CurrentCard cc = db.CurrentCards.Find(id);
            if (cc == null)
            {
                return Redirect("/CurrentCards/Index");
            }
            return View(cc);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CurrentCard cc = db.CurrentCards.Find(id);
            CurrentCardDetail cd = db.CurrentCardDetails.Find(id);

            db.CurrentCardDetails.Remove(cd);

            db.CurrentCards.Remove(cc);
            db.SaveChanges();

            return Redirect("/CurrentCards/Index");
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