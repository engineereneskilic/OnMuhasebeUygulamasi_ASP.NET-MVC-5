using OnMuhasebeUygulamasi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnMuhasebeUygulamasi.MultipleModelView
{
    public class CurrentCardswithAC
    {
        public List<CurrentCard> CurrentCardList { get; set; }
        public List<AccountMovement> AccountMovementList { get; set; }
    }
}