using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnMuhasebeUygulamasi.Models;
using System.Web.Mvc;
using PagedList;

namespace OnMuhasebeUygulamasi.MultipleModelView
{
    public class BillBigModel {

        public List<Bill> BillList { get; set; }
        public IPagedList<Bill> BillListPL { get; set; }
        public List<BillDetail> BillDetailList { get; set; }
        public BillDetail SelectedBilDetail { get; set; }
        public Bill SelectedBilll { get; set; }

        public Stock SelectedStock { get; set; }

        public List<CurrentCard> CurrentCardList { get; set; }
        public Bill newBill { get; set; }

        public List<Stock> StockList { get; set; }
    }
}