using OnMuhasebeUygulamasi.Models;
using OnMuhasebeUygulamasi.MultipleModelView;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnMuhasebeUygulamasi.Controllers
{
    public class StockController : Controller
    {
        private PreliminaryAccountingEntities db = new PreliminaryAccountingEntities();

        // GET: Stock
        public ActionResult Index(int? id)
        {
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            var fullstocklist =
                from st in db.Stocks
                select st;



            //db.SaveChanges(); // kaydedilimki yeni anabirim girişini sağlamış olalım
            //Stoğa anabirim cinsinden girişleri güncelle
            (from sm in db.Stocks select sm).ToList().ForEach(m => m.StockMainUnit = 0);





            // Stoğa anabirim cinsinden girişleri güncelle
            (from sm in db.StockMovements
             join s in db.Stocks on
              sm.StockCode 
             equals
              s.StockCode
             select new { s, sm }).Where(m => m.sm.IncomingAmount !=null).ToList().ForEach(m => m.s.StockMainUnit += m.sm.IncomingAmount);

            db.SaveChanges(); // kaydedilimki yeni anabirim girişini sağlamış olalım

            //Stoğa anabirim cinsinden çıkışları güncelle
            (from sm in db.StockMovements
             join s in db.Stocks on
              sm.StockCode
             equals
             s.StockCode
            select new { s, sm }).Where(m => m.sm.Yield !=null).ToList().ForEach(m => m.s.StockMainUnit -= m.sm.Yield);

            db.SaveChanges(); // girmiş olan değerlerden ne kadar çıkmasını gerektiiğinide söyledikten sonra son durumu kaydedelim

            //if (User.Identity.Name == "") return RedirectToAction("LogOn", "Account"); else return View(stocklist.ToList());

            //if (Request.QueryString["sh"] != null && Request.QueryString["sh"] != "")
            //{
            //    int selected_sh_id = int.Parse(Request.QueryString["sh"]);

            //    var sh_stockmovements =
            //    from sh in db.StockMovements
            //    where sh.StockCode == selected_sh_id
            //    select sh;

            //    StockwithStockDetails swsd = new StockwithStockDetails()
            //    {
            //        StockList = fullstocklist.ToList(),
            //        StockMovementList = sh_stockmovements.ToList()
            //    };

            //    return View(swsd);
            //}
            if (User.Identity.Name == "") return RedirectToAction("LogOn", "Account");
            else
            {

                if (id == null) // id yoksa faturaları listele
                {

                    StockwithStockDetails swsd = new StockwithStockDetails()
                    {
                        StockList = fullstocklist.ToList()

                    };

                    return View(swsd);

                }
                else
                {
                    var sh_stockmovements =
                from sh in db.StockMovements
                where sh.StockCode == id
                select sh;

                    StockwithStockDetails swsd = new StockwithStockDetails()
                    {
                        StockList = fullstocklist.ToList(),
                        StockMovementList = sh_stockmovements.ToList()

                    };
                    return View(swsd);
                }

            }
        }
    
        // GET: Stocks/Create
        public ActionResult Create()
        {
            return View();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Stock st)
        {
            if (ModelState.IsValid)
            {
                db.Stocks.Add(st);
                db.SaveChanges();

                return RedirectToAction("Index");
            }


            return View(st);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return Redirect("/Stock/Index");
            }
            Stock s = db.Stocks.Find(id);
            if (s == null)
            {
                return Redirect("/Stock/Index");
            }
            return View(s);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return Redirect("/Stock/Index");
            }

            Stock cc = db.Stocks.Find(id);
            if (cc == null)
            {
                return Redirect("/Stock/Index");
            }
            return View(cc);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Stock s)
        {

            if (ModelState.IsValid)
            {
                db.Entry(s).State = EntityState.Modified;
                db.SaveChanges();


                return RedirectToAction("Index");
            }

            return View(s);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return Redirect("/Stock/Index");
            }
            Stock s = db.Stocks.Find(id);
            if (s == null)
            {
                return Redirect("/Stock/Index");
            }
            return View(s);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stock s = db.Stocks.Find(id);

            db.Stocks.Remove(s);
            db.SaveChanges();

            return RedirectToAction("Index");
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