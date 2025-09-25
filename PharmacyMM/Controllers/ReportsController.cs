using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using ClosedXML.Excel;
using PharmacyMM.Models;

namespace PharmacyMM.Controllers
{
    public class ReportsController : Controller
    {
        private PharmacyDBEntities db = new PharmacyDBEntities();

        // GET: Reports/Index (Overview with links to specific reports)
        public ActionResult Index()
        {
            return View();
        }

        // GET: Reports/Stock
        public ActionResult Stock(int? lowStockThreshold = 10)
        {
            var medicines = db.Medicines
                .Select(m => new StockReportViewModel
                {
                    MedicineID = m.MedicineID,
                    MedicineName = m.MedicineName,
                    StockQuantity = m.StockQuantity ?? 0,
                    UnitPrice = m.UnitPrice,
                    ExpiryDate = m.ExpiryDate
                })
                .ToList();

            ViewBag.LowStockThreshold = lowStockThreshold;
            ViewBag.LowStockItems = medicines.Where(m => m.StockQuantity < lowStockThreshold).ToList();

            return View(medicines);
        }

        // GET: Reports/Profit
        public ActionResult Profit(DateTime? startDate = null, DateTime? endDate = null)
        {
            // Use provided dates or default to last month if null
            startDate = startDate ?? DateTime.Now.AddMonths(-1);
            endDate = endDate ?? DateTime.Now;

            // Calculate profit: (SaleDetails.UnitPrice - Medicine.UnitPrice) * SaleDetails.Quantity
            var salesData = db.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .Join(db.SaleDetails,
                    sale => sale.SaleID,
                    saleDetail => saleDetail.SaleID,
                    (sale, saleDetail) => new { Sale = sale, SaleDetail = saleDetail })
                .Join(db.Medicines,
                    s => s.SaleDetail.MedicineID,
                    medicine => medicine.MedicineID,
                    (s, medicine) => new
                    {
                        SalePrice = s.SaleDetail.UnitPrice,
                        CostPrice = medicine.UnitPrice,
                        Quantity = s.SaleDetail.Quantity,
                        TotalSales = s.Sale.TotalAmount
                    })
                .ToList();

            var totalSales = salesData.Sum(s => s.TotalSales);
            var totalProfit = salesData.Sum(s => (s.SalePrice - s.CostPrice) * s.Quantity);

            var model = new ProfitReportViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalSales = totalSales,
                TotalPurchases = 0, // Not used in this profit calculation
                Profit = totalProfit
            };

            return View(model);
        }

        // GET: Reports/Expiration
        public ActionResult Expiration(int? daysThreshold = 30)
        {
            var thresholdDate = DateTime.Now.AddDays(daysThreshold.Value);
            var medicines = db.Medicines
                .Where(m => m.ExpiryDate <= thresholdDate && m.ExpiryDate >= DateTime.Now && (m.StockQuantity ?? 0) > 0)
                .OrderBy(m => m.ExpiryDate)
                .Select(m => new ExpirationReportViewModel
                {
                    MedicineID = m.MedicineID,
                    MedicineName = m.MedicineName,
                    StockQuantity = m.StockQuantity ?? 0,
                    ExpiryDate = m.ExpiryDate,
                    DaysToExpiry = 0 // Set to 0 initially, will calculate in memory
                })
                .ToList()
                .Select(m => new ExpirationReportViewModel
                {
                    MedicineID = m.MedicineID,
                    MedicineName = m.MedicineName,
                    StockQuantity = m.StockQuantity,
                    ExpiryDate = m.ExpiryDate,
                    DaysToExpiry = m.ExpiryDate != null ? (int)(m.ExpiryDate.Value - DateTime.Now).TotalDays : 0
                })
                .ToList();

            ViewBag.DaysThreshold = daysThreshold;
            ViewBag.UrgentItems = medicines.Where(m => m.DaysToExpiry <= 7).ToList();

            return View(medicines);
        }

        public ActionResult Report(string period = "daily", DateTime? selectedDate = null)
        {
            // Default to today if no date provided
            selectedDate = selectedDate ?? DateTime.Now;

            // Determine date range based on period
            DateTime startDate;
            DateTime endDate;
            string weekStart = null, weekEnd = null, month = null;

            switch (period.ToLower())
            {
                case "daily":
                    startDate = selectedDate.Value.Date;
                    endDate = startDate.AddDays(1).AddTicks(-1); // End of the day
                    break;
                case "weekly":
                    startDate = selectedDate.Value.Date.AddDays(-(int)selectedDate.Value.DayOfWeek); // Start of week (Sunday)
                    endDate = startDate.AddDays(7).AddTicks(-1); // End of week
                    weekStart = startDate.ToString("yyyy-MM-dd");
                    weekEnd = endDate.ToString("yyyy-MM-dd");
                    break;
                case "monthly":
                    startDate = new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, 1); // Start of month
                    endDate = startDate.AddMonths(1).AddTicks(-1); // End of month
                    month = startDate.ToString("MMMM yyyy");
                    break;
                default:
                    startDate = selectedDate.Value.Date;
                    endDate = startDate.AddDays(1).AddTicks(-1);
                    period = "daily";
                    break;
            }

            // Calculate TotalRevenue, TotalCost, and Profit
            var salesData = db.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .Join(db.SaleDetails,
                    sale => sale.SaleID,
                    saleDetail => saleDetail.SaleID,
                    (sale, saleDetail) => new { Sale = sale, SaleDetail = saleDetail })
                .Join(db.Medicines,
                    s => s.SaleDetail.MedicineID,
                    medicine => medicine.MedicineID,
                    (s, medicine) => new
                    {
                        SalePrice = s.SaleDetail.UnitPrice,
                        CostPrice = medicine.UnitPrice,
                        Quantity = s.SaleDetail.Quantity,
                        TotalSales = s.Sale.TotalAmount
                    })
                .ToList();

            var totalRevenue = salesData.Sum(s => s.TotalSales);
            var totalCost = salesData.Sum(s => s.CostPrice * s.Quantity);
            var profit = totalRevenue - totalCost;

            // Set ViewBag properties for the view
            ViewBag.Period = period;
            ViewBag.SelectedDate = selectedDate.Value.ToString("yyyy-MM-dd");
            ViewBag.WeekStart = weekStart;
            ViewBag.WeekEnd = weekEnd;
            ViewBag.Month = month;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalCost = totalCost;
            ViewBag.Profit = profit;

            return View();
        }



        //public FileResult DownloadUsersReport()
        //{
        //    using (var workbook = new ClosedXML.Excel.XLWorkbook())
        //    {
        //        var ws = workbook.Worksheets.Add("Users Report");
        //        ws.Cell(1, 1).Value = "Username";
        //        ws.Cell(1, 2).Value = "Full Name";
        //        ws.Cell(1, 3).Value = "Role";
        //        ws.Cell(1, 4).Value = "Phone";
        //        ws.Cell(1, 5).Value = "Licence";

        //        var users = db.Users.ToList();
        //        int row = 2;
        //        foreach (var u in users)
        //        {
        //            ws.Cell(row, 1).Value = u.Username;
        //            ws.Cell(row, 2).Value = u.full_name;
        //            ws.Cell(row, 3).Value = u.Role;
        //            ws.Cell(row, 4).Value = u.phone;
        //            ws.Cell(row, 5).Value = u.Licence;
        //            row++;
        //        }

        //        using (var ms = new MemoryStream())
        //        {
        //            workbook.SaveAs(ms);
        //            return File(ms.ToArray(),
        //                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                        "UsersReport.xlsx");
        //        }
        //    }
        //}

        public FileResult DownloadFullReport()
        {
            using (var workbook = new XLWorkbook())
            {
                // =====================
                // 1️⃣ Stock / Expiration Sheet
                // =====================
                var wsStock = workbook.Worksheets.Add("Stock & Expiration");

                // Header
                wsStock.Cell(1, 1).Value = "Medicine Name";
                wsStock.Cell(1, 2).Value = "Stock Quantity";
                wsStock.Cell(1, 3).Value = "Unit Price";
                wsStock.Cell(1, 4).Value = "Expiry Date";
                wsStock.Cell(1, 5).Value = "Days to Expiry";

                var headerRange = wsStock.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Data
                var medicines = db.Medicines.ToList();
                int row = 2;
                foreach (var m in medicines)
                {
                    wsStock.Cell(row, 1).Value = m.MedicineName;
                    wsStock.Cell(row, 2).Value = m.StockQuantity ?? 0;
                    wsStock.Cell(row, 3).Value = m.UnitPrice ;
                    wsStock.Cell(row, 4).Value = m.ExpiryDate?.ToString("yyyy-MM-dd");
                    wsStock.Cell(row, 5).Value = m.ExpiryDate != null ? (m.ExpiryDate.Value - DateTime.Now).Days : 0;

                    // Highlight low stock
                    if ((m.StockQuantity ?? 0) < 10)
                    {
                        wsStock.Row(row).Style.Fill.BackgroundColor = XLColor.LightYellow;
                    }

                    // Highlight soon-to-expire
                    if (m.ExpiryDate != null && (m.ExpiryDate.Value - DateTime.Now).Days <= 7)
                    {
                        wsStock.Row(row).Style.Fill.BackgroundColor = XLColor.LightPink;
                    }

                    row++;
                }

                wsStock.Columns().AdjustToContents();

                // =====================
                // 2️⃣ Profit Sheet (Daily Only)
                // =====================
                var wsProfit = workbook.Worksheets.Add("Daily Profit");

                // Header
                wsProfit.Cell(1, 1).Value = "Sale Date";
                wsProfit.Cell(1, 2).Value = "Medicine Name";
                wsProfit.Cell(1, 3).Value = "Quantity Sold";
                wsProfit.Cell(1, 4).Value = "Sale Price";
                wsProfit.Cell(1, 5).Value = "Cost Price";
                wsProfit.Cell(1, 6).Value = "Profit";

                var profitHeader = wsProfit.Range(1, 1, 1, 6);
                profitHeader.Style.Font.Bold = true;
                profitHeader.Style.Fill.BackgroundColor = XLColor.LightGreen;
                profitHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var today = DateTime.Now.Date;
                var tomorrow = today.AddDays(1);

                var profitData = db.Sales
                    .Where(s => s.SaleDate >= today && s.SaleDate < tomorrow)
                    .Join(db.SaleDetails, s => s.SaleID, sd => sd.SaleID, (s, sd) => new { s, sd })
                    .Join(db.Medicines, x => x.sd.MedicineID, m => m.MedicineID, (x, m) => new
                    {
                        SaleDate = x.s.SaleDate,
                        MedicineName = m.MedicineName,
                        Quantity = x.sd.Quantity ,
                        SalePrice = x.sd.UnitPrice ,
                        CostPrice = m.UnitPrice ,
                        Profit = ((x.sd.UnitPrice ) - (m.UnitPrice )) * (x.sd.Quantity)
                    })
                    .ToList();

                row = 2;
                foreach (var p in profitData)
                {
                    wsProfit.Cell(row, 1).Value = p.SaleDate.ToString("yyyy-MM-dd");
                    wsProfit.Cell(row, 2).Value = p.MedicineName;
                    wsProfit.Cell(row, 3).Value = p.Quantity;
                    wsProfit.Cell(row, 4).Value = p.SalePrice;
                    wsProfit.Cell(row, 5).Value = p.CostPrice;
                    wsProfit.Cell(row, 6).Value = p.Profit;

                    // Highlight profitable rows
                    if (p.Profit > 0)
                    {
                        wsProfit.Row(row).Style.Fill.BackgroundColor = XLColor.AliceBlue;
                    }

                    row++;
                }

                wsProfit.Columns().AdjustToContents();

                // =====================
                // Save to MemoryStream and return file
                // =====================
                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    return File(ms.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                $"DailyProfitReport_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
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

    public class StockReportViewModel
    {
        public int MedicineID { get; set; }
        public string MedicineName { get; set; }
        public int StockQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class ProfitReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; } // Kept for compatibility, not used
        public decimal Profit { get; set; }
    }

    public class ExpirationReportViewModel
    {
        public int MedicineID { get; set; }
        public string MedicineName { get; set; }
        public int StockQuantity { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int DaysToExpiry { get; set; }
    }
}