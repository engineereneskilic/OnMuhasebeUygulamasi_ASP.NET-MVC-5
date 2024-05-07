using OnMuhasebeUygulamasi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnMuhasebeUygulamasi.MultipleModelView
{
    public class StockwithStockDetails
    {
        public List<Stock> StockList { get; set; }
        public List<StockMovement> StockMovementList { get; set; }
    }
}