using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using OnMuhasebeUygulamasi.Models;
using System.Dynamic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Data;
using System.Text;
using System.Globalization;

namespace OnMuhasebeUygulamasi.Controllers
{
    public class ReportController : Controller
    {

        public static  DataTable ToDataTable(IEnumerable<dynamic> items)
        {
            var data = items.ToArray();
            if (data.Count() == 0) return null;

            var dt = new DataTable();
            foreach (var key in ((IDictionary<string, object>)data[0]).Keys)
            {
                dt.Columns.Add(key);
            }
            foreach (var d in data)
            {
                dt.Rows.Add(((IDictionary<string, object>)d).Values.ToArray());
            }
            return dt;
        }

        private PreliminaryAccountingEntities db = new PreliminaryAccountingEntities();

        static DateTime now = DateTime.Now;
        static DateTime OneMonthLater = DateTime.Today.AddMonths(+1);
        static DateTime ThreeMonthLater = DateTime.Today.AddMonths(+3);
        TimeSpan timeFormat = new TimeSpan(now.Hour, now.Minute, now.Second);

        dynamic model = new ExpandoObject();



        // GET: Report
        public ActionResult Index()
        {
            
            if (User.Identity.IsAuthenticated) ViewBag.Role = db.aspnet_Users.Where(au => au.UserName == User.Identity.Name).FirstOrDefault().RoleID.ToString();

            gettReport1();
            gettReport2();
            gettReport3();
            gettReport4();
            gettReport5();

            return View(model);
        }

       
       
        public ActionResult ExportToExcel(int? id)
        {
            var gv = new GridView();

            //GridView1.Columns.Add(sutun1);

            switch (id)
            {
                case 1:
                    gettReport1();
                    gv.DataSource =  ToDataTable(model.Report1);
                    break;
                case 2:
                    gettReport2();
                    gv.DataSource = ToDataTable(model.Report2);
                    break;
                case 3:
                    gettReport3();
                    gv.DataSource = ToDataTable(model.Report3);
                    break;
                case 4:
                    gettReport4();
                    gv.DataSource = ToDataTable(model.Report4);
                    break;
                case 5:
                    gettReport5();
                    gv.DataSource = ToDataTable(model.Report5);
                    break;
            }
    
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report"+id+".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

           return RedirectToAction("Index");
        }
        

        public void gettReport1()
        {

            var report1 = db.AccountMovements.GroupBy(day => new { y = day.MovementDate.Value.Year, m = day.MovementDate.Value.Month, d = day.MovementDate.Value.Day }).ToList()
                .Select(
                day =>
                new
                {

                    Date = new DateTime(day.Key.y, day.Key.m, day.Key.d),
                    TotalCostProfit = (day.Sum(s => s.Debt) - day.Sum(s => s.Credit)),
                    TotalSumDebt = day.Sum(s => s.Debt),
                    TotalSumCredit = day.Sum(s => s.Credit)
                });

            List<ExpandoObject> joinData = new List<ExpandoObject>();

            foreach (var item in report1)
            {

                IDictionary<string, object> itemExpando = new ExpandoObject();
                foreach (PropertyDescriptor property
                         in
                         TypeDescriptor.GetProperties(item.GetType()))
                {
                    itemExpando.Add(property.Name, property.GetValue(item));
                }
                joinData.Add(itemExpando as ExpandoObject);
            }

            model.Report1 = joinData;
        }

        public void gettReport2()
        {

            var report2 = db.StockMovements
            .Join(db.Stocks,
                sm => sm.StockCode,
                s => s.StockCode,
                (sm, s) => new { sm, s }).ToList() // selection
            .Where(m => m.sm.ProcessDate.Value.Year == now.Year && m.sm.ProcessDate.Value.Month == now.Month).ToList()    // where statement

            .Select(
            s =>
            new
            {
                StockBarcode = s.s.StockBarcode,
                StockName = trytoeng(s.s.StockName),
                StockComment = trytoeng(s.s.StockComment),
                StockType = trytoeng(s.s.StockType),
                StockMainUnit = s.s.StockMainUnit,
                StockMainUnitBuyingPrice = s.s.StockMainUnitBuyingPrice,
                IncomingAmount = s.sm.IncomingAmount,
                StockMainUnitType = trytoeng(s.s.StockMainUnitType),
                TotalAmount = s.sm.TotalAmount

            }).OrderByDescending(o => o.IncomingAmount).Take(3).ToList();

            
            List<ExpandoObject> joinData = new List<ExpandoObject>();

            foreach (var item in report2)
            {

                IDictionary<string, object> itemExpando = new ExpandoObject();
                foreach (PropertyDescriptor property
                         in
                         TypeDescriptor.GetProperties(item.GetType()))
                {
                    itemExpando.Add(property.Name, property.GetValue(item));
                }
                joinData.Add(itemExpando as ExpandoObject);
            }

            model.Report2 = joinData;
            
        }

        public void gettReport3()
        {

            var report3 = db.AccountMovements
           .Join(db.CurrentCards,
               am => am.CurrentCode,
               cc => cc.CurrentCode,
               (am, cc) => new { am, cc })
           .Where(m => m.am.MovementDate.Value.Year == OneMonthLater.Year && m.am.MovementDate.Value.Month == OneMonthLater.Month).ToList()    // where statement
           .GroupBy(g => new { g.am.CurrentCode, g.cc.CurrentName })
           .Select(
           s =>
           new
           {
               CurrentName = trytoeng(s.Key.CurrentName),
               TotalDebt = s.Sum(sm => sm.am.Debt),
               TotalCredit = s.Sum(sm => sm.am.Credit)

           }).OrderByDescending(o => o.TotalCredit).ToList();


            List<ExpandoObject> joinData = new List<ExpandoObject>();

            foreach (var item in report3)
            {

                IDictionary<string, object> itemExpando = new ExpandoObject();
                foreach (PropertyDescriptor property
                         in
                         TypeDescriptor.GetProperties(item.GetType()))
                {
                    itemExpando.Add(property.Name, property.GetValue(item));
                }
                joinData.Add(itemExpando as ExpandoObject);
            }

            model.Report3 = joinData;
        }

        public void gettReport4()
        {

            var report4 = db.StockMovements
           .Join(db.Stocks,
               sm => sm.StockCode,
               s => s.StockCode,
               (sm, s) => new { sm, s })
           .Where(m => m.sm.ProcessDate.Value.Year == now.Year && 11 >= m.sm.ProcessDate.Value.Month)    // where statement
           .GroupBy(g => new { m = g.sm.ProcessDate.Value.Month, sc = g.sm.StockCode}).ToList()
           .Select(
           s =>
           new
           {
               Month = new DateTime(now.Year, s.Key.m, 1),
               StockName = trytoeng(db.Stocks.Where( sw => sw.StockCode == s.Key.sc).FirstOrDefault().StockName),
               Yield = s.Sum(y => y.sm.Yield),
               TotalAmount = s.Sum(sm => sm.sm.TotalAmount),


           }).OrderByDescending(od => od.TotalAmount).Take(3).ToList();


            List<ExpandoObject> joinData = new List<ExpandoObject>();

            foreach (var item in report4)
            {

                IDictionary<string, object> itemExpando = new ExpandoObject();
                foreach (PropertyDescriptor property
                         in
                         TypeDescriptor.GetProperties(item.GetType()))
                {
                    itemExpando.Add(property.Name, property.GetValue(item));
                }
                joinData.Add(itemExpando as ExpandoObject);
            }

            model.Report4 = joinData;
        }

        public void gettReport5()
        {

            var report5 = db.AccountMovements
           .Join(db.CurrentCards,
               am=> am.CurrentCode,
               c => c.CurrentCode,
               (am, c) => new { am, c }).Join(db.CurrentCardDetails,
               m => m.am.CurrentCode,
               ccd => ccd.CurrentCode,
               (m, ccd) => new { m, ccd })
           .Where(m => m.m.am.MovementDate.Value.Year == OneMonthLater.Year && OneMonthLater.Month== m.m.am.MovementDate.Value.Month)    // where statement
           .GroupBy(g => new {cc = g.m.am.CurrentCode,CurrentName=g.m.c.CurrentName, SecondPhone = g.ccd.SecondPhone,FirstInformation=g.ccd.FirstInformation,GsmNo=g.ccd.GsmNo,Email=g.ccd.Email,Address= g.ccd.Address,Province = g.ccd.Province,District=g.ccd.District }).ToList()
           .Select(
           s =>
           new
           {
               CurrentName = trytoeng(s.Key.CurrentName),
               FirstInformation =  trytoeng(s.Key.FirstInformation),
               GsmNo = s.Key.GsmNo,
               Email = s.Key.Email,
               Adress =  trytoeng(s.Key.Address),
               Province = trytoeng(s.Key.Province),
               District = trytoeng(s.Key.District),
               
               Credit = s.Sum(cd => cd.m.am.Credit),

           }).OrderByDescending(od => od.Credit).Take(4).ToList();


            List<ExpandoObject> joinData = new List<ExpandoObject>();

            foreach (var item in report5)
            {

                IDictionary<string, object> itemExpando = new ExpandoObject();
                foreach (PropertyDescriptor property
                         in
                         TypeDescriptor.GetProperties(item.GetType()))
                {
                    itemExpando.Add(property.Name, property.GetValue(item));
                }
                joinData.Add(itemExpando as ExpandoObject);
            }

            model.Report5 = joinData;
        }
        public string trytoeng(string text)
        {
            text=text.Replace("Ü", "U");text=text.Replace("ü", "u");
            text=text.Replace("İ", "I"); text = text.Replace("ı", "i");
            text = text.Replace("Ö", "O"); text = text.Replace("ö", "o");
            text = text.Replace("Ş", "S"); text = text.Replace("ş","s");
            text = text.Replace("Ç", "C"); text = text.Replace("ç", "c");
            /*
            string unaccentedText = String.Join("", text.Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
            */
            return text;
        }
    }
}