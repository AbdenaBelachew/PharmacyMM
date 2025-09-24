using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmacyMM.Models
{
    public class SaleDetailViewModel
    {
        public int MedicineID { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;
    }
}