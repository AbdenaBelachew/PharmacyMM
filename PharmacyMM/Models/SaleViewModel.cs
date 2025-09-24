using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PharmacyMM.Models
{
    public class SaleViewModel
    {
        public int SaleID { get; set; }

        [Display(Name = "Customer")]
        public int? CustomerID { get; set; }
        public IEnumerable<Customer> Customers { get; set; }

        [Display(Name = "Sale Date")]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        public List<SaleDetailViewModel> SaleDetails { get; set; } = new List<SaleDetailViewModel>();

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
    }
}