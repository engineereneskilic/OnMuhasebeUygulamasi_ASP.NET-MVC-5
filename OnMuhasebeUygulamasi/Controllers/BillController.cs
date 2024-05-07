using OnMuhasebeUygulamasi.Models;
using OnMuhasebeUygulamasi.MultipleModelView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace OnMuhasebeUygulamasi.Controllers
{
    public class BillController : Controller
    {
        private PreliminaryAccountingEntities db = new PreliminaryAccountingEntities();

        static DateTime now = DateTime.Now;
        TimeSpan timeFormat = new TimeSpan(now.Hour, now.Minute, now.Second);
        int page = 1;
        int pageSize = 10;

        public void Pagging()
        {
            if (Request.QueryString["page"] != null)
            {
                page = Convert.ToInt32(Request.QueryString["page"]);
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
        }


        // GET: Bill
        public ActionResult Index(int? id)
        {
            Pagging();
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();
            // tam liste
            var fullbilllist = db.Bills.ToList();

          
            PagedList<Bill> fullbilllistPL = new PagedList<Bill>(fullbilllist, page, pageSize);


            var CurrentCardList = db.CurrentCards.ToList();



            

            if (Request.QueryString["ds"] != null && Request.QueryString["ds"] != "") // mevcut fatura detayı kaydı arayüzü
            {
                string ds = Request.QueryString["ds"].Replace("%20", " ");

                BillBigModel bwbd = new BillBigModel()
                {
                    BillList = fullbilllist,
                    BillListPL = fullbilllistPL,
                    SelectedBilll = db.Bills.Where(sb => sb.BillCode == id).FirstOrDefault(),
                    SelectedStock = db.Stocks.Where(item => item.StockName == ds).FirstOrDefault(),
                    StockList = db.Stocks.ToList(),
                    CurrentCardList = db.CurrentCards.ToList()
                };
                return View(bwbd);
            }else

            if (Request.QueryString["bd"] != null && Request.QueryString["bd"] != "") // fatura detayı arayüzü
            {
                int selected_db_id = int.Parse(Request.QueryString["bd"]);

                var selectedbildetail =
                (from bd in db.BillDetails
                 where bd.BillDetailID == selected_db_id
                 select bd).FirstOrDefault();

                var fullbildetailsllist =
                        from bl in db.BillDetails
                        where bl.BillCode == id
                        select bl;


                // seçilen fatura detayına bağlı stokCode colununa görede bağlı olduğu stock modelini alalım..
                var selectedStock = (from ss in db.Stocks where ss.StockCode == selectedbildetail.StockCode select ss).FirstOrDefault();



                BillBigModel bwbd = new BillBigModel()
                {
                    BillList = fullbilllist,
                    BillListPL = fullbilllistPL,
                    BillDetailList = fullbildetailsllist.ToList(),
                    SelectedBilll = db.Bills.Where(sb => sb.BillCode == id).FirstOrDefault(),
                    SelectedBilDetail = selectedbildetail,
                    SelectedStock = selectedStock,
                    StockList = db.Stocks.ToList(),
                    CurrentCardList = db.CurrentCards.ToList()
                };

                return View(bwbd);

            } else

            if(Request.QueryString["ds"] != null && Request.QueryString["ds"] != "" && Request.QueryString["bd"] != null && Request.QueryString["bd"] != "")
            {
                var fullbildetailsllist =
                        from bl in db.BillDetails
                        where bl.BillCode == id
                        select bl;

                string ds = Request.QueryString["ds"].Replace("%20", " ");

                BillBigModel bwbd = new BillBigModel()
                {
                    BillList = fullbilllist,
                    BillListPL = fullbilllistPL,
                    BillDetailList = fullbildetailsllist.ToList(),
                    SelectedBilll = db.Bills.Where(sb => sb.BillCode == id).FirstOrDefault(),
                    SelectedStock = db.Stocks.Where(item => item.StockName == ds).FirstOrDefault(),
                    StockList = db.Stocks.ToList(),
                    CurrentCardList = db.CurrentCards.ToList()
                };
                return View(bwbd);
            }

            else
            if (Request.QueryString["newbd"] == "true") // yeni fatura detayı kaydı arayüzü
            {
                var fullbildetailsllist =
                        from bl in db.BillDetails
                        where bl.BillCode == id
                        select bl;

                var selectedStock = (from ss in db.Stocks where ss.StockCode == 1 select ss).FirstOrDefault();

                BillBigModel bwbd = new BillBigModel()
                {
                    BillList = fullbilllist,
                    BillListPL = fullbilllistPL,
                    BillDetailList = fullbildetailsllist.ToList(),
                    SelectedBilll = db.Bills.FirstOrDefault(), //  gizli kısım default modeli birinci olsun zaten gizli duruma göre değişecek
                    SelectedStock = selectedStock,
                    StockList = db.Stocks.ToList(),
                    CurrentCardList = db.CurrentCards.ToList()
                };

                return View(bwbd);
            }




            if (User.Identity.Name == "") return RedirectToAction("LogOn", "Account");
            else
            {

                if (id == null) // id yoksa faturaları listele
                {

                    BillBigModel bwbd = new BillBigModel()
                    {
                        BillList = fullbilllist,
                        BillListPL = fullbilllistPL,
                        CurrentCardList = CurrentCardList.ToList(),
                        SelectedBilll= db.Bills.Where(sbd => sbd.BillCode==1).FirstOrDefault(),

                    };

                    return View(bwbd);

                }
                else // Index uzerine id gelmesi fatura detayı isteği geldi demek gelen fatura idsine bağlı tüm fatura elemanlarını alalım.
                {
                    var bildetailsllist =
                from bl in db.BillDetails
                where bl.BillCode == id
                select bl;


                    BillBigModel bwbd = new BillBigModel()
                    {
                        BillList = fullbilllist,
                        BillListPL = fullbilllistPL,
                        BillDetailList = bildetailsllist.ToList(),
                        CurrentCardList = CurrentCardList.ToList(),
                        SelectedBilll = db.Bills.Where(sb =>sb.BillCode == id).FirstOrDefault(),
                    };

                    return View(bwbd);
                }



            }



        }
        // GET: Categories/Edit/5
        //public ActionResult Edit(int? id)
        //{

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    BillDetail bd = db.BillDetails.Find(id);
        //    if (bd == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bd);
        //}

        double newBillTotalAmount = 0;
        double? newBillTotalDiscount = 0;
        double? newBillTotalKdv = 0;
       int girencikan = 0;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BillBigModel bwbd)
        {
            Pagging();
            //  CurrentCard cc = db.CurrentCards.Find(cd.CurrentCode);



            var BillDetails = from bd in db.BillDetails
                             where bd.BillCode == bwbd.SelectedBilDetail.BillCode
                             group bd by bd.BillDetailID into g
                             select new
                             {

                             };
            if (ModelState.IsValid)
            {
                if (bwbd.BillList != null)
                {
                    db.Entry(bwbd.BillList).State = EntityState.Modified;
                }
                else
                if (bwbd.BillDetailList != null)
                {
                    db.Entry(bwbd.BillDetailList).State = EntityState.Modified;
                }
                else
                if (bwbd.SelectedBilDetail != null)
                {

                    // Fatura Güncellemesi Başı

                    Bill updateBill =
                    (from bl in db.Bills
                     where bl.BillCode == bwbd.SelectedBilDetail.BillCode
                     select bl).SingleOrDefault();

                   

                    db.Entry(bwbd.SelectedBilDetail).State = EntityState.Modified;



                    if (bwbd.SelectedBilDetail.IncomingAmount != null) girencikan = Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount); else girencikan = Convert.ToInt32(bwbd.SelectedBilDetail.Yield);

                    Stock selectedStock = (from ss in db.Stocks where ss.StockCode == bwbd.SelectedBilDetail.StockCode select ss).FirstOrDefault();
                    bwbd.SelectedBilDetail.Discript = selectedStock.StockName;

                    newBillTotalAmount = (db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode).Select(sm => sm.LineTotalAmount).Sum()).Value;



              
                    
                    //Discount Güncellemesi

                    if(((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : ((bb.UnitPrice * bb.IncomingAmount) * (bb.DiscountPercent / 100))).Sum())) != null)
                      {
                        newBillTotalDiscount = newBillTotalDiscount + ((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : ((bb.UnitPrice * bb.IncomingAmount) * (bb.DiscountPercent / 100))).Sum()));
                      }
                    
                    if(((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : ((bb.UnitPrice * bb.Yield) * (bb.DiscountPercent / 100))).Sum())) != null)
                    {
                        newBillTotalDiscount = newBillTotalDiscount + ((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : ((bb.UnitPrice * bb.Yield) * (bb.DiscountPercent / 100))).Sum()));
                    }
                   
                    //KDV Güncellemesi

                    if (((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : (bb.UnitPrice * bb.IncomingAmount) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.IncomingAmount))).Sum()) != null)
                    {
                        newBillTotalKdv = newBillTotalKdv + ((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : (bb.UnitPrice * bb.IncomingAmount) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.IncomingAmount))).Sum());
                    }

                    if (((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : (bb.UnitPrice * bb.Yield) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.Yield))).Sum() != null))
                    {
                        newBillTotalKdv = newBillTotalKdv + ((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : (bb.UnitPrice * bb.Yield) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.Yield))).Sum());
                    }

                    //newBillTotalKdv = (db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode).Select(sm => (((sm.UnitPrice * sm.IncomingAmount) + (sm.UnitPrice * sm.Yield)) * (1+(sm.KdvPercent/100))) - ((sm.UnitPrice * sm.IncomingAmount) + (sm.UnitPrice * sm.Yield))).Sum()).Value;

                    updateBill.ProcessDate = Convert.ToDateTime(now.ToShortDateString());
                    updateBill.ProcessTime = timeFormat;
                    updateBill.ProcessDate = Convert.ToDateTime(now);



                    if (updateBill.ProcessType == "Alış Faturası") { updateBill.NetCredit = newBillTotalAmount; } else updateBill.NetDebt = newBillTotalAmount;
                    updateBill.Discount = newBillTotalDiscount;
                    updateBill.Kdv = newBillTotalKdv;

                    db.Entry(updateBill).State = EntityState.Modified;

                    // FATURA GÜNCELLEMESİ SONU
                    // STOK HAREKETİ GÜNCELLEMESİ BAŞLASIN
                    StockMovement smm = (from sm in db.StockMovements where sm.SequanceNo== bwbd.SelectedBilDetail.BillCode && sm.StockCode==bwbd.SelectedBilDetail.StockCode select sm).FirstOrDefault();
                    smm.ProcessDate = Convert.ToDateTime(now.ToShortDateString());

                    smm.ProcessType = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().ProcessType;
                    smm.CurrentCode = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().CurrrentCode;
                    smm.CurrentName = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().CurrentName;
                    smm.StockCode = bwbd.SelectedBilDetail.StockCode;

                    //Giren-Çıkan Miktar birinci birime dönüştürülür vee Stok hareketi kısmındada gerekli güncelleme sağlansın
                    //Giren Miktar Dönüşümü
                    if (bwbd.SelectedBilDetail.IncomingAmount != null)
                    {
                        if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockMainUnitType)
                        {
                            selectedStock.StockMainUnit += Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount);
                            smm.IncomingAmount = Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount);
                        }
                        else
                        if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockSecondUnitType)
                        {
                            selectedStock.StockMainUnit += Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount) * Convert.ToInt32(selectedStock.StockSecondUnit);
                            smm.IncomingAmount = Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount) * Convert.ToInt32(selectedStock.StockSecondUnit);
                        }
                        else
                        if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockThirdUnitType)
                        {
                            selectedStock.StockMainUnit += Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount) * Convert.ToInt32(selectedStock.StockThirdUnit);
                            smm.IncomingAmount = Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount) * Convert.ToInt32(selectedStock.StockThirdUnit);
                        }
                    }
                    else
                    if (bwbd.SelectedBilDetail.Yield != null)
                    {
                        if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockMainUnitType)
                        {
                            selectedStock.StockMainUnit -= Convert.ToInt32(bwbd.SelectedBilDetail.Yield);
                            smm.Yield = Convert.ToInt32(bwbd.SelectedBilDetail.Yield);
                        }
                        else
                       if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockSecondUnitType)
                        {
                            selectedStock.StockMainUnit -= Convert.ToInt32(bwbd.SelectedBilDetail.Yield) * Convert.ToInt32(selectedStock.StockSecondUnit);
                            smm.Yield = Convert.ToInt32(bwbd.SelectedBilDetail.Yield) * Convert.ToInt32(selectedStock.StockSecondUnit);
                        }
                        else
                       if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockThirdUnitType)
                        {
                            selectedStock.StockMainUnit -= Convert.ToInt32(bwbd.SelectedBilDetail.Yield) * Convert.ToInt32(selectedStock.StockThirdUnit);
                            smm.Yield = Convert.ToInt32(bwbd.SelectedBilDetail.Yield) * Convert.ToInt32(selectedStock.StockThirdUnit);
                        }
                    }
                    // DİĞER DEĞERLERİN GÜNCELLENMESİ

                    smm.UnitPrice = bwbd.SelectedBilDetail.UnitPrice;

                    smm.TotalAmount = bwbd.SelectedBilDetail.LineTotalAmount;

                    smm.Discount =  Convert.ToInt32((girencikan * bwbd.SelectedBilDetail.UnitPrice) * (bwbd.SelectedBilDetail.DiscountPercent / 100));

                    smm.SequanceNo = bwbd.SelectedBilDetail.BillCode;

                    db.Entry(smm).State = EntityState.Modified;
                    db.Entry(selectedStock).State = EntityState.Modified;
                    // STOK HAREKETİ SONU

                    //Cari kart hesap hareketi güncellemesi
                    AccountMovement am = (from amm in db.AccountMovements where amm.SequenceNo == updateBill.BillCode select amm).FirstOrDefault();
                    am.MovementDate = Convert.ToDateTime(now.ToShortDateString());
                    am.SequenceNo = bwbd.SelectedBilDetail.BillCode;
                    am.ProcessType = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().ProcessType;
                    am.Descript = updateBill.Description;
                    am.Credit = updateBill.NetCredit;
                    am.Debt = updateBill.NetDebt;
                    am.Currency = updateBill.Currency;
                    am.CurrentCode = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().CurrrentCode;

                    db.Entry(am).State = EntityState.Modified;

                    // Carikart gelir-gider güncellemesi
                    CurrentCard cd = (from cdd in db.CurrentCards where cdd.CurrentCode == updateBill.CurrrentCode select cdd).FirstOrDefault();
                    cd.BalanceCredit = updateBill.NetCredit;
                    cd.BalanceDebt = updateBill.NetDebt;
                    cd.Currency = updateBill.Currency;

                    db.Entry(cd).State = EntityState.Modified;

                }else
                
                if(bwbd.SelectedBilll != null)
                {
                    Bill updateBill = db.Bills.Where(up => up.BillCode == bwbd.SelectedBilll.BillCode).FirstOrDefault();
                    //List<BillDetail> updateBillDetailList = db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilll.BillCode).ToList();
                    updateBill.ProcessDate = Convert.ToDateTime(now.ToShortDateString());
                    updateBill.ProcessTime = timeFormat;

                    //1.ADIM
                    // ProcessType'da DEĞİŞİKLİK OLURSA

                    updateBill.ProcessType = bwbd.SelectedBilll.ProcessType;
                    
                    // borç alacak ilişkisi güncellemesi
                    if (updateBill.ProcessType == "Alış Faturası") if (updateBill.NetDebt != null) { updateBill.NetCredit = updateBill.NetDebt; updateBill.NetDebt = null; }
                    if (updateBill.ProcessType == "Satış Faturası") if (updateBill.NetCredit != null) { updateBill.NetDebt = updateBill.NetCredit; updateBill.NetCredit = null; }


                    db.Entry(updateBill).State = EntityState.Modified;

                    // ProcessType Göre Giren Çıkar değerleri güncelle
                    db.BillDetails.Where(_bdl => _bdl.BillCode == bwbd.SelectedBilll.BillCode && bwbd.SelectedBilll.ProcessType == "Alış Faturası" && _bdl.Yield != null).ToList().ForEach(_abdl=> _abdl.IncomingAmount=_abdl.Yield);
                    db.BillDetails.Where(_bdl => _bdl.BillCode == bwbd.SelectedBilll.BillCode && bwbd.SelectedBilll.ProcessType == "Alış Faturası").ToList().ForEach(_abdl => _abdl.Yield=null);

                    db.BillDetails.Where(_bdl => _bdl.BillCode == bwbd.SelectedBilll.BillCode && bwbd.SelectedBilll.ProcessType == "Satış Faturası" && _bdl.IncomingAmount != null).ToList().ForEach(_abdl => _abdl.Yield = _abdl.IncomingAmount);
                    db.BillDetails.Where(_bdl => _bdl.BillCode == bwbd.SelectedBilll.BillCode && bwbd.SelectedBilll.ProcessType == "Satış Faturası").ToList().ForEach(_abdl => _abdl.IncomingAmount = null);

                    // Stok hareketi giriş çıkışları güncellenmiş
                    (from sm in db.StockMovements
                                     join bd in db.BillDetails on
                                     new {d=sm.SequanceNo,c= sm.StockCode }
                                     equals
                                     new {d= bd.BillCode ,c=bd.StockCode }
                                     select new { sm,bd}).Where(m => m.bd.BillCode  == bwbd.SelectedBilll.BillCode).ToList().ForEach(m => m.sm.IncomingAmount = m.bd.IncomingAmount);
                    (from sm in db.StockMovements
                     join bd in db.BillDetails on
                     new { d = sm.SequanceNo, c = sm.StockCode }
                     equals
                     new { d = bd.BillCode, c = bd.StockCode }
                     select new { sm, bd }).Where(m => m.bd.BillCode == bwbd.SelectedBilll.BillCode).ToList().ForEach(m => m.sm.Yield= m.bd.Yield);

                    (from sm in db.StockMovements
                     join bd in db.BillDetails on
                     new { d = sm.SequanceNo, c = sm.StockCode }
                     equals
                     new { d = bd.BillCode, c = bd.StockCode }
                     select new { sm, bd }).Where(m => m.bd.BillCode == bwbd.SelectedBilll.BillCode).ToList().ForEach(m => m.sm.ProcessDate= updateBill.ProcessDate);

                    (from sm in db.StockMovements
                     join bd in db.BillDetails on
                     new { d = sm.SequanceNo, c = sm.StockCode }
                     equals
                     new { d = bd.BillCode, c = bd.StockCode }
                     select new { sm, bd }).Where(m => m.bd.BillCode == bwbd.SelectedBilll.BillCode).ToList().ForEach(m => m.sm.ProcessType = updateBill.ProcessType);

                    //Cari kartlar hareketi düzenlemesi
                    AccountMovement am = db.AccountMovements.Where(amm => amm.SequenceNo == updateBill.BillCode).FirstOrDefault();
                    if (am != null)
                    { // daha önceden hareketi varsa yoksa bu yeni bir faturadır
                        am.MovementDate = updateBill.ProcessDate;
                        am.ProcessType = updateBill.ProcessType;
                        am.Descript = updateBill.Description;

                        if (am.ProcessType == "Alış Faturası") if (am.Debt != null && am.Debt!=0) { am.Credit = am.Debt; am.Debt = 0; }
                        if (am.ProcessType == "Satış Faturası") if (am.Credit != null && am.Credit!=0) { am.Debt = am.Credit; am.Credit = 0; }
                        am.Currency = updateBill.Currency;
                    }
                    db.Entry(am).State = EntityState.Modified;

                    //2.ADIM
                    //CURRENTNAME DE DEĞİŞİKLŞK OLURSA
                    updateBill.CurrentName = bwbd.SelectedBilll.CurrentName;
                    //Stok Hareketleri güncellendi
                    db.StockMovements.Where(s => s.SequanceNo == bwbd.SelectedBilll.BillCode).ToList().ForEach(upcn => upcn.CurrentName = bwbd.SelectedBilll.CurrentName);

                    db.StockMovements.Where(s => s.SequanceNo == bwbd.SelectedBilll.BillCode).ToList().ForEach(upcc => upcc.CurrentCode = db.CurrentCards.Where(cdid => cdid.CurrentName == bwbd.SelectedBilll.CurrentName).FirstOrDefault().CurrentCode);

                     AccountMovement cmAm= db.AccountMovements.Where(ammm => ammm.SequenceNo == bwbd.SelectedBilll.BillCode).FirstOrDefault();
                    cmAm.CurrentCode = db.CurrentCards.Where(cdid => cdid.CurrentName == bwbd.SelectedBilll.CurrentName).FirstOrDefault().CurrentCode;

                    db.Entry(cmAm).State = EntityState.Modified;

                    //3.ADIM
                    //Term Değişiklik olursa
                    updateBill.Term = bwbd.SelectedBilll.Term;


                    db.SaveChanges();



                    return Redirect("/Bill/Index/" + bwbd.SelectedBilll.BillCode + "?page=" + page + "&pageSize=" + pageSize);

                }

                //Where(_sm => _sm.SequanceNo == bwbd.SelectedBilll.BillCode && _sm.StockCode== updateBillDetailList.ToList().Select(x => x.StockCode));


                else
                if (bwbd.BillList != null && bwbd.BillDetailList != null && bwbd.SelectedBilDetail != null)
                {
                    db.Entry(bwbd).State = EntityState.Modified;
                }

                db.SaveChanges();

                return Redirect("/Bill/Index/" + bwbd.SelectedBilDetail.BillCode + "?page=" + page + "&pageSize=" + pageSize);
            }
            else
            {
                return View();
            }

        }


        //// GET: Products/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CurrentCard cc = db.CurrentCards.Find(id);
        //    if (cc == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cc);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Pagging();
            Bill SelectedBill = db.Bills.Where(sb => sb.BillCode == id).FirstOrDefault();
            //1.adım faturayı sil

            db.Bills.Remove(SelectedBill);

            //2.adım fatura detayını sil
            List<BillDetail> deleteBillDetail = db.BillDetails.Where(bd => bd.BillCode == SelectedBill.BillCode).ToList();
            db.BillDetails.RemoveRange(deleteBillDetail);

            //3.adım stok hareketlerini sil Not: Stok sayfası yüklendiğinde kendisi sürekli giriş-çıkış güncellemesi yapyıyor bu nedenle stoğa müdahale etmek istemiyorum buradan sağlıklı olmuyor
            List<StockMovement> deleteStockMovement = db.StockMovements.Where(sm => sm.SequanceNo == SelectedBill.BillCode).ToList();
            db.StockMovements.RemoveRange(deleteStockMovement);
            

            //4.adım cari hareketlerden sil Not: Cari kartlara dokunmiyacağım cari kartlar sayfası açıldığında son duruma göre kendi güncellemelerini yapsın
            AccountMovement deleteAccountMovement = db.AccountMovements.Where(am => am.SequenceNo == SelectedBill.BillCode).FirstOrDefault();
            db.AccountMovements.Remove(deleteAccountMovement);

            db.SaveChanges();

            return RedirectToAction("Index", "Bill");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBd(int id)
        {
            Pagging();
            BillDetail SelectedBillDetail = db.BillDetails.Where(bd => bd.BillDetailID == id).FirstOrDefault();

            db.BillDetails.Remove(SelectedBillDetail);

            StockMovement deleteStockMovement = db.StockMovements.Where(sm => sm.SequanceNo == SelectedBillDetail.BillCode && sm.StockCode == SelectedBillDetail.StockCode).FirstOrDefault();
            db.StockMovements.Remove(deleteStockMovement);


            db.SaveChanges();

            // Silme işlemi bitti gerekli olan tüm güncellemeleri yapalım

            // Fatura Güncellemesi Başı

            Bill updateBill =
            (from bl in db.Bills
             where bl.BillCode == SelectedBillDetail.BillCode
             select bl).SingleOrDefault();




            if (SelectedBillDetail.IncomingAmount != null) girencikan = Convert.ToInt32(SelectedBillDetail.IncomingAmount); else girencikan = Convert.ToInt32(SelectedBillDetail.Yield);

            Stock selectedStock = (from ss in db.Stocks where ss.StockCode == SelectedBillDetail.StockCode select ss).FirstOrDefault();
            SelectedBillDetail.Discript = selectedStock.StockName;

            newBillTotalAmount = (db.BillDetails.Where(bd => bd.BillCode == SelectedBillDetail.BillCode).Select(sm => sm.LineTotalAmount).Sum()).Value;





            //Discount Güncellemesi

            if (((db.BillDetails.Where(bd => bd.BillCode ==SelectedBillDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : ((bb.UnitPrice * bb.IncomingAmount) * (bb.DiscountPercent / 100))).Sum())) != null)
            {
                newBillTotalDiscount = newBillTotalDiscount + ((db.BillDetails.Where(bd => bd.BillCode == SelectedBillDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : ((bb.UnitPrice * bb.IncomingAmount) * (bb.DiscountPercent / 100))).Sum()));
            }

            if (((db.BillDetails.Where(bd => bd.BillCode == SelectedBillDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : ((bb.UnitPrice * bb.Yield) * (bb.DiscountPercent / 100))).Sum())) != null)
            {
                newBillTotalDiscount = newBillTotalDiscount + ((db.BillDetails.Where(bd => bd.BillCode == SelectedBillDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : ((bb.UnitPrice * bb.Yield) * (bb.DiscountPercent / 100))).Sum()));
            }

            //KDV Güncellemesi

            if (((db.BillDetails.Where(bd => bd.BillCode == SelectedBillDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : (bb.UnitPrice * bb.IncomingAmount) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.IncomingAmount))).Sum()) != null)
            {
                newBillTotalKdv = newBillTotalKdv + ((db.BillDetails.Where(bd => bd.BillCode == SelectedBillDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : (bb.UnitPrice * bb.IncomingAmount) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.IncomingAmount))).Sum());
            }

            if (((db.BillDetails.Where(bd => bd.BillCode == SelectedBillDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : (bb.UnitPrice * bb.Yield) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.Yield))).Sum() != null))
            {
                newBillTotalKdv = newBillTotalKdv + ((db.BillDetails.Where(bd => bd.BillCode == SelectedBillDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : (bb.UnitPrice * bb.Yield) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.Yield))).Sum());
            }

            //newBillTotalKdv = (db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode).Select(sm => (((sm.UnitPrice * sm.IncomingAmount) + (sm.UnitPrice * sm.Yield)) * (1+(sm.KdvPercent/100))) - ((sm.UnitPrice * sm.IncomingAmount) + (sm.UnitPrice * sm.Yield))).Sum()).Value;

            updateBill.ProcessDate = Convert.ToDateTime(now.ToShortDateString());
            updateBill.ProcessTime = timeFormat;
            updateBill.ProcessDate = Convert.ToDateTime(now);



            if (updateBill.ProcessType == "Alış Faturası") { updateBill.NetCredit = newBillTotalAmount; } else updateBill.NetDebt = newBillTotalAmount;
            updateBill.Discount = newBillTotalDiscount;
            updateBill.Kdv = newBillTotalKdv;

            db.Entry(updateBill).State = EntityState.Modified;

            // FATURA GÜNCELLEMESİ SONU
            //Cari kart güncellemesi
            AccountMovement Upam = db.AccountMovements.Where(c => c.SequenceNo == SelectedBillDetail.BillCode).FirstOrDefault();
            Upam.MovementDate = Convert.ToDateTime(now.ToShortDateString());
            Upam.Debt = updateBill.NetDebt;
            Upam.Credit = updateBill.NetCredit;

            db.Entry(Upam).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("Index", "Bill");
        }

        /*
        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CurrentCardDetail ccdt = db.CurrentCardDetails.Find(id);
            if (ccdt == null)
            {
                return HttpNotFound();
            }
            return View(ccdt);
        }
        */


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BillBigModel bwbd)
        {
            Pagging();
            if (bwbd.newBill != null)
            {

                bwbd.newBill.ProcessDate = Convert.ToDateTime(now.ToShortDateString());
                bwbd.newBill.ProcessTime = timeFormat;
                bwbd.newBill.CurrrentCode = db.CurrentCards.Where(cd => cd.CurrentName == bwbd.newBill.CurrentName).FirstOrDefault().CurrentCode;
                db.Bills.Add(bwbd.newBill);
                db.SaveChanges();

                AccountMovement am = new AccountMovement();
                am.MovementDate = Convert.ToDateTime(now.ToShortDateString());
                am.SequenceNo = bwbd.newBill.BillCode;
                am.ProcessType = bwbd.newBill.ProcessType;
                am.Descript = bwbd.newBill.Description;
                am.Currency = bwbd.newBill.Currency;
                am.CurrentCode = db.CurrentCards.Where(cd => cd.CurrentName == bwbd.newBill.CurrentName).FirstOrDefault().CurrentCode;

                //aynı faturayı bir daha ekleme
                //Bill duplicatebill = db.bi enes


                db.AccountMovements.Add(am);
                db.SaveChanges();

                return Redirect("/Bill/Index/" + bwbd.newBill.BillCode+"?page="+page+"&pageSize="+pageSize);
            }
            else
            if (bwbd.SelectedBilDetail!=null)
            {
                // Fatura Ekleme Başı

                Bill updateBill =
                (from bl in db.Bills
                 where bl.BillCode == bwbd.SelectedBilDetail.BillCode
                 select bl).SingleOrDefault();

                Stock selectedStock = (from ss in db.Stocks where ss.StockCode == bwbd.SelectedBilDetail.StockCode select ss).FirstOrDefault();
                bwbd.SelectedBilDetail.Discript = selectedStock.StockName;


              

                

                if (bwbd.SelectedBilDetail.IncomingAmount != null) girencikan = Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount); else girencikan = Convert.ToInt32(bwbd.SelectedBilDetail.Yield);


                if((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.LineTotalAmount != null).Select(sm => sm.LineTotalAmount == null ? 0 : sm.LineTotalAmount).Sum()) != null)
                {
                    newBillTotalAmount = (db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode).Select(sm => sm.LineTotalAmount).Sum()).Value;
                }

                





                //Discount Güncellemesi

                if (((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : ((bb.UnitPrice * bb.IncomingAmount) * (bb.DiscountPercent / 100))).Sum())) != null)
                {
                    newBillTotalDiscount = newBillTotalDiscount + ((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : ((bb.UnitPrice * bb.IncomingAmount) * (bb.DiscountPercent / 100))).Sum()));
                }

                if (((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : ((bb.UnitPrice * bb.Yield) * (bb.DiscountPercent / 100))).Sum())) != null)
                {
                    newBillTotalDiscount = newBillTotalDiscount + ((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : ((bb.UnitPrice * bb.Yield) * (bb.DiscountPercent / 100))).Sum()));
                }

                //KDV Güncellemesi

                if (((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : (bb.UnitPrice * bb.IncomingAmount) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.IncomingAmount))).Sum()) != null)
                {
                    newBillTotalKdv = newBillTotalKdv + ((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.IncomingAmount != null).Select(bb => bb.IncomingAmount == null ? 0 : (bb.UnitPrice * bb.IncomingAmount) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.IncomingAmount))).Sum());
                }

                if (((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : (bb.UnitPrice * bb.Yield) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.Yield))).Sum() != null))
                {
                    newBillTotalKdv = newBillTotalKdv + ((db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode && bd.Yield != null).Select(bb => bb.Yield == null ? 0 : (bb.UnitPrice * bb.Yield) * (1 + (bb.KdvPercent / 100)) - (bb.UnitPrice * bb.Yield))).Sum());
                }

                //newBillTotalKdv = (db.BillDetails.Where(bd => bd.BillCode == bwbd.SelectedBilDetail.BillCode).Select(sm => (((sm.UnitPrice * sm.IncomingAmount) + (sm.UnitPrice * sm.Yield)) * (1+(sm.KdvPercent/100))) - ((sm.UnitPrice * sm.IncomingAmount) + (sm.UnitPrice * sm.Yield))).Sum()).Value;

                updateBill.ProcessDate = Convert.ToDateTime(now.ToShortDateString());
                updateBill.ProcessTime = timeFormat;
                updateBill.ProcessDate = Convert.ToDateTime(now);



                if (updateBill.ProcessType == "Alış Faturası") { updateBill.NetCredit = newBillTotalAmount; } else updateBill.NetDebt = newBillTotalAmount;
                updateBill.Discount = newBillTotalDiscount;
                updateBill.Kdv = newBillTotalKdv;

                db.Entry(updateBill).State = EntityState.Modified;
                db.BillDetails.Add(bwbd.SelectedBilDetail);

                // FATURA GÜNCELLEMESİ SONU
                // STOK HAREKETi ekleme BAŞLASIN
                StockMovement smm = new StockMovement();
                smm.ProcessDate = Convert.ToDateTime(now.ToShortDateString());
                smm.ProcessType = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().ProcessType;
                smm.CurrentCode = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().CurrrentCode;
                smm.CurrentName = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().CurrentName;
                smm.StockCode = bwbd.SelectedBilDetail.StockCode;


                //Giren-Çıkan Miktar birinci birime dönüştürülür vee Stok hareketi kısmındada gerekli güncelleme sağlansın
                //Giren Miktar Dönüşümü
                if (bwbd.SelectedBilDetail.IncomingAmount != null)
                {
                    if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockMainUnitType)
                    {
                        selectedStock.StockMainUnit += Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount);
                        smm.IncomingAmount = Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount);
                    }
                    else
                    if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockSecondUnitType)
                    {
                        selectedStock.StockMainUnit += Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount) * Convert.ToInt32(selectedStock.StockSecondUnit);
                        smm.IncomingAmount = Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount) * Convert.ToInt32(selectedStock.StockSecondUnit);
                    }
                    else
                    if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockThirdUnitType)
                    {
                        selectedStock.StockMainUnit += Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount) * Convert.ToInt32(selectedStock.StockThirdUnit);
                        smm.IncomingAmount = Convert.ToInt32(bwbd.SelectedBilDetail.IncomingAmount) * Convert.ToInt32(selectedStock.StockThirdUnit);
                    }
                }
                else
                if (bwbd.SelectedBilDetail.Yield != null)
                {
                    if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockMainUnitType)
                    {
                        selectedStock.StockMainUnit -= Convert.ToInt32(bwbd.SelectedBilDetail.Yield);
                        smm.Yield = Convert.ToInt32(bwbd.SelectedBilDetail.Yield);
                    }
                    else
                   if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockSecondUnitType)
                    {
                        selectedStock.StockMainUnit -= Convert.ToInt32(bwbd.SelectedBilDetail.Yield) * Convert.ToInt32(selectedStock.StockSecondUnit);
                        smm.Yield = Convert.ToInt32(bwbd.SelectedBilDetail.Yield) * Convert.ToInt32(selectedStock.StockSecondUnit);
                    }
                    else
                   if (bwbd.SelectedBilDetail.UnitType == selectedStock.StockThirdUnitType)
                    {
                        selectedStock.StockMainUnit -= Convert.ToInt32(bwbd.SelectedBilDetail.Yield) * Convert.ToInt32(selectedStock.StockThirdUnit);
                        smm.Yield = Convert.ToInt32(bwbd.SelectedBilDetail.Yield) * Convert.ToInt32(selectedStock.StockThirdUnit);
                    }
                }
                // DİĞER DEĞERLERİN GÜNCELLENMESİ

                smm.UnitPrice = bwbd.SelectedBilDetail.UnitPrice;

                smm.TotalAmount = bwbd.SelectedBilDetail.LineTotalAmount;

                smm.Discount = Convert.ToInt32((girencikan * bwbd.SelectedBilDetail.UnitPrice) * (bwbd.SelectedBilDetail.DiscountPercent / 100));

                smm.SequanceNo = bwbd.SelectedBilDetail.BillCode;

                db.StockMovements.Add(smm);
                db.Entry(selectedStock).State = EntityState.Modified;
                
                // STOK HAREKETİ SONU

                //Cari kart hesap hareketi güncellemesi
                AccountMovement am = (from amm in db.AccountMovements where amm.SequenceNo == updateBill.BillCode select amm).FirstOrDefault();
                am.MovementDate = Convert.ToDateTime(now.ToShortDateString());
                am.SequenceNo = bwbd.SelectedBilDetail.BillCode;
                am.ProcessType = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().ProcessType;
                am.Descript = updateBill.Description;
                am.Credit = updateBill.NetCredit;
                am.Debt = updateBill.NetDebt;
                am.Currency = updateBill.Currency;
                am.CurrentCode = db.Bills.Where(b => b.BillCode == bwbd.SelectedBilDetail.BillCode).FirstOrDefault().CurrrentCode;

                db.Entry(am).State = EntityState.Modified;

                // Carikart gelir-gider güncellemesi
                CurrentCard cd = (from cdd in db.CurrentCards where cdd.CurrentCode == updateBill.CurrrentCode select cdd).FirstOrDefault();
                cd.BalanceCredit = updateBill.NetCredit;
                cd.BalanceDebt = updateBill.NetDebt;
                cd.Currency = updateBill.Currency;

                db.Entry(cd).State = EntityState.Modified;

                db.SaveChanges();

                return Redirect("/Bill/Index/" + bwbd.SelectedBilDetail.BillCode + "?page=" + page + "&pageSize=" + pageSize);
            }

            //db.CurrentCards.Add(cd.CurrentCard);
            //db.SaveChanges();


            return Redirect("/Bill/Index/" + bwbd.SelectedBilDetail.BillCode + "?page=" + page + "&pageSize=" + pageSize);
        }
    }
}