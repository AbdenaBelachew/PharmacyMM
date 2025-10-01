using System;
using System.Linq;
using System.Web.Mvc;
using PharmacyMM.Models;

namespace PharmacyMM.Controllers
{
    public class HomeController : Controller
    {
        private PharmacyDBEntities db = new PharmacyDBEntities();

        public ActionResult Index()
        {
            // ✅ Total Revenue (from Sales)
            var totalRevenue = db.Sales.Sum(s => (decimal?)s.TotalAmount) ?? 0;

         
            // ✅ Total Products
            var totalProducts = db.Medicines.Count();

            // ✅ Low Stock (reusing Stock report idea)
            var lowStockItems = db.Medicines
                .Where(m => (m.StockQuantity ?? 0) < 10)
                .Count();

            // ✅ Soon Expiring Medicines (from Expiration report idea)
            var thresholdDate = DateTime.Now.AddDays(30);
            var soonExpiring = db.Medicines
                .Where(m => m.ExpiryDate <= thresholdDate && m.ExpiryDate >= DateTime.Now && (m.StockQuantity ?? 0) > 0)
                .Count();

            // ✅ Monthly sales (for Chart.js bar chart)
            var monthlySales = db.Sales
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Total = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            // Send to ViewBag
            ViewBag.TotalRevenue = totalRevenue;
         
           
            ViewBag.TotalProducts = totalProducts;
            ViewBag.LowStock = lowStockItems;
            ViewBag.SoonExpiring = soonExpiring;

            // For charts
            ViewBag.MonthlySalesLabels = string.Join(",", monthlySales.Select(m => $"'{m.Month}/{m.Year}'"));
            ViewBag.MonthlySalesData = string.Join(",", monthlySales.Select(m => m.Total));

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
