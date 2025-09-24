using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PharmacyMM.Models;

namespace PharmacyMM.Controllers
{
    public class PurchasesController : Controller
    {
        private PharmacyDBEntities db = new PharmacyDBEntities();

        //        // GET: Purchases
        //        public ActionResult Index()
        //        {
        //            var purchases = db.Purchases.Include(p => p.Supplier).OrderByDescending(p => p.PurchaseDate);
        //            return View(purchases.ToList());
        //        }

        //        // GET: Purchases/Details/5
        //        public ActionResult Details(int? id)
        //        {
        //            if (id == null)
        //            {
        //                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //            }
        //            Purchase purchase = db.Purchases
        //                .Include(p => p.Supplier)
        //                .Include(p => p.PurchaseDetails)

        //                .FirstOrDefault(p => p.PurchaseID == id);
        //            if (purchase == null)
        //            {
        //                return HttpNotFound();
        //            }
        //            return View(purchase);
        //        }

        //        // GET: Purchases/Purchase
        //        public ActionResult Purchase()
        //        {
        //            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName");
        //            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName");
        //            ViewBag.Medicines = db.Medicines.ToList();
        //            return View(new PurchaseViewModel());
        //        }

        //        // POST: Purchases/Purchase
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public ActionResult Purchase(PurchaseViewModel model)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                using (var transaction = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        var medicine = db.Medicines.Find(model.MedicineID);
        //                        if (medicine == null)
        //                        {
        //                            ModelState.AddModelError("", "Invalid medicine.");
        //                            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", model.SupplierID);
        //                            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName", model.MedicineID);
        //                            ViewBag.Medicines = db.Medicines.ToList();
        //                            return View(model);
        //                        }
        //                        if (model.UnitPrice <= 0)
        //                        {
        //                            ModelState.AddModelError("UnitPrice", "Unit price must be greater than zero.");
        //                            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", model.SupplierID);
        //                            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName", model.MedicineID);
        //                            ViewBag.Medicines = db.Medicines.ToList();
        //                            return View(model);
        //                        }

        //                        var purchase = new Purchase
        //                        {
        //                            SupplierID = model.SupplierID,
        //                            PurchaseDate = DateTime.Now,
        //                            TotalAmount = model.Quantity * model.UnitPrice
        //                        };

        //                        db.Purchases.Add(purchase);
        //                        db.SaveChanges();

        //                        var purchaseDetail = new PurchaseDetail
        //                        {
        //                            PurchaseID = purchase.PurchaseID,
        //                            MedicineID = model.MedicineID,
        //                            Quantity = model.Quantity,
        //                            UnitPrice = model.UnitPrice
        //                        };

        //                        medicine.StockQuantity += model.Quantity;
        //                        db.PurchaseDetails.Add(purchaseDetail);
        //                        db.SaveChanges();

        //                        transaction.Commit();
        //                        return RedirectToAction("Index");
        //                    }
        //                    catch
        //                    {
        //                        transaction.Rollback();
        //                        ModelState.AddModelError("", "An error occurred while processing the purchase.");
        //                    }
        //                }
        //            }

        //            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", model.SupplierID);
        //            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName", model.MedicineID);
        //            ViewBag.Medicines = db.Medicines.ToList();
        //            return View(model);
        //        }

        //        // GET: Purchases/MultiplePurchase
        //        public ActionResult MultiplePurchase()
        //        {
        //            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName");
        //            ViewBag.Medicines = db.Medicines.ToList();
        //            var model = new MultiplePurchaseViewModel { PurchaseItems = new List<PurchaseItemViewModel> { new PurchaseItemViewModel() } };
        //            return View(model);
        //        }

        //        // POST: Purchases/MultiplePurchase
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public ActionResult MultiplePurchase(MultiplePurchaseViewModel model)
        //        {
        //            if (ModelState.IsValid && model.PurchaseItems != null && model.PurchaseItems.Any())
        //            {
        //                using (var transaction = db.Database.BeginTransaction())
        //                {
        //                    try
        //                    {
        //                        decimal totalAmount = 0;
        //                        var purchase = new Purchase
        //                        {
        //                            SupplierID = model.SupplierID,
        //                            PurchaseDate = DateTime.Now,
        //                            TotalAmount = 0
        //                        };

        //                        db.Purchases.Add(purchase);
        //                        db.SaveChanges();

        //                        foreach (var item in model.PurchaseItems)
        //                        {
        //                            if (item.Quantity <= 0 || item.UnitPrice <= 0) continue;

        //                            var medicine = db.Medicines.Find(item.MedicineID);
        //                            if (medicine == null)
        //                            {
        //                                ModelState.AddModelError("", $"Invalid medicine ID: {item.MedicineID}.");
        //                                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", model.SupplierID);
        //                                ViewBag.Medicines = db.Medicines.ToList();
        //                                transaction.Rollback();
        //                                return View(model);
        //                            }

        //                            var purchaseDetail = new PurchaseDetail
        //                            {
        //                                PurchaseID = purchase.PurchaseID,
        //                                MedicineID = item.MedicineID,
        //                                Quantity = item.Quantity,
        //                                UnitPrice = item.UnitPrice
        //                            };

        //                            totalAmount += purchaseDetail.Quantity * purchaseDetail.UnitPrice;
        //                            medicine.StockQuantity += item.Quantity;
        //                            db.PurchaseDetails.Add(purchaseDetail);
        //                        }

        //                        if (totalAmount == 0)
        //                        {
        //                            ModelState.AddModelError("", "No valid purchase items were provided.");
        //                            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", model.SupplierID);
        //                            ViewBag.Medicines = db.Medicines.ToList();
        //                            transaction.Rollback();
        //                            return View(model);
        //                        }

        //                        purchase.TotalAmount = totalAmount;
        //                        db.SaveChanges();
        //                        transaction.Commit();
        //                        return RedirectToAction("Index");
        //                    }
        //                    catch
        //                    {
        //                        transaction.Rollback();
        //                        ModelState.AddModelError("", "An error occurred while processing the purchase.");
        //                    }
        //                }
        //            }

        //            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", model.SupplierID);
        //            ViewBag.Medicines = db.Medicines.ToList();
        //            return View(model);
        //        }

        //        protected override void Dispose(bool disposing)
        //        {
        //            if (disposing)
        //            {
        //                db.Dispose();
        //            }
        //            base.Dispose(disposing);
        //        }
        //    }

        //    public class PurchaseViewModel
        //    {
        //        public int SupplierID { get; set; }
        //        public int MedicineID { get; set; }
        //        public int Quantity { get; set; }
        //        public decimal UnitPrice { get; set; }
        //    }

        //    public class PurchaseItemViewModel
        //    {
        //        public int MedicineID { get; set; }
        //        public int Quantity { get; set; }
        //        public decimal UnitPrice { get; set; }
        //    }

        //    public class MultiplePurchaseViewModel
        //    {
        //        public int SupplierID { get; set; }
        //        public List<PurchaseItemViewModel> PurchaseItems { get; set; }
        //    }
        //}

        // GET: Purchases
        public ActionResult Index()
        {
            var purchases = db.Purchases.Include(p => p.Supplier).OrderByDescending(p => p.PurchaseDate);
            return View(purchases.ToList());
        }

        // GET: Purchases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var purchase = db.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                .FirstOrDefault(p => p.PurchaseID == id);

            if (purchase == null) return HttpNotFound();

            return View(purchase);
        }

        // GET: Purchases/Purchase
        public ActionResult Purchase()
        {
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName");
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName"); // <-- Fix here
            ViewBag.Medicines = db.Medicines.ToList();
            return View(new PurchaseViewModel());
        }

        // POST: Purchases/Purchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(PurchaseViewModel model)
        {
            var username = Session["Username"];
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Check if medicine exists by name
                        var medicine = db.Medicines.FirstOrDefault(m => m.MedicineName == model.MedicineName.Trim());

                        if (medicine == null)
                        {
                            // Create new medicine
                            medicine = new Medicine
                            {
                                CreatedBy = username.ToString(),
                                MedicineName = model.MedicineName.Trim(),
                                StockQuantity = model.Quantity,
                                UnitPrice = model.UnitPrice,
                                CategoryID = model.CategoryID,
                                Manufacturer = model.Manufacturer,
                                ExpiryDate = model.ExpiryDate
                            };
                            db.Medicines.Add(medicine);
                        }
                        else
                        {
                            // Existing medicine → update stock and optionally update details
                            medicine.CreatedBy = username.ToString();
                            medicine.StockQuantity += model.Quantity;
                            medicine.UnitPrice = model.UnitPrice;           // optional
                            medicine.ExpiryDate = model.ExpiryDate;        // optional: update if provided
                            medicine.CategoryID = model.CategoryID;        // optional: update if changed
                            medicine.Manufacturer = model.Manufacturer;    // optional: update if changed
                        }

                        // Save medicine first to get MedicineID
                        db.SaveChanges();

                        // Create purchase
                        var purchase = new Purchase
                        {
                            SupplierID = model.SupplierID,
                            PurchaseDate = DateTime.Now,
                            TotalAmount = model.Quantity * model.UnitPrice
                        };
                        db.Purchases.Add(purchase);
                        db.SaveChanges();

                        // Create purchase detail
                        var purchaseDetail = new PurchaseDetail
                        {
                            PurchaseID = purchase.PurchaseID,
                            MedicineID = medicine.MedicineID,
                            Quantity = model.Quantity,
                            UnitPrice = model.UnitPrice
                        };
                        db.PurchaseDetails.Add(purchaseDetail);
                        db.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "An error occurred while processing the purchase.");
                    }
                }
            }

            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", model.SupplierID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", model.CategoryID);
            return View(model);
        }


        // GET: Purchases/MultiplePurchase
        public ActionResult MultiplePurchase()
        {
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName");
            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CategoryName");

            var model = new MultiplePurchaseViewModel
            {
                PurchaseItems = new List<PurchaseItemViewModel> { new PurchaseItemViewModel() }
            };
            return View(model);
        }

        // POST: Purchases/MultiplePurchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MultiplePurchase(MultiplePurchaseViewModel model)
        {
            var username = Session["Username"];
            if (!ModelState.IsValid || model.PurchaseItems == null || !model.PurchaseItems.Any())
            {
                ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", model.SupplierID);
                ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CategoryName");
                return View(model);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    decimal totalAmount = 0;

                    var purchase = new Purchase
                    {
                        SupplierID = model.SupplierID,
                        PurchaseDate = DateTime.Now,
                        TotalAmount = 0
                    };
                    db.Purchases.Add(purchase);
                    db.SaveChanges();

                    foreach (var item in model.PurchaseItems)
                    {
                        if (item.Quantity <= 0 || item.UnitPrice <= 0) continue;

                        // Check if medicine exists
                        var existingMedicine = db.Medicines.FirstOrDefault(m => m.MedicineName == item.MedicineName);
                        if (existingMedicine != null)
                        {
                            existingMedicine.CreatedBy = username.ToString();
                            existingMedicine.StockQuantity += item.Quantity;
                            existingMedicine.UnitPrice = item.UnitPrice;
                            existingMedicine.ExpiryDate = item.ExpiryDate;
                            existingMedicine.Manufacturer = item.Manufacturer;    // <-- use item.Manufacturer
                            existingMedicine.CategoryID = item.CategoryID;       // <-- use item.CategoryID
                            db.Entry(existingMedicine).State = System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            existingMedicine = new Medicine
                            {
                                CreatedBy = username.ToString(),
                                MedicineName = item.MedicineName,
                                UnitPrice = item.UnitPrice,
                                StockQuantity = item.Quantity,
                                ExpiryDate = item.ExpiryDate,
                                Manufacturer = item.Manufacturer,    // <-- use item.Manufacturer
                                CategoryID = item.CategoryID         // <-- use item.CategoryID
                            };
                            db.Medicines.Add(existingMedicine);
                            db.SaveChanges();
                        }

                        var purchaseDetail = new PurchaseDetail
                        {
                            PurchaseID = purchase.PurchaseID,
                            MedicineID = existingMedicine.MedicineID,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        };
                        totalAmount += purchaseDetail.Quantity * purchaseDetail.UnitPrice;
                        db.PurchaseDetails.Add(purchaseDetail);
                    }


                    purchase.TotalAmount = totalAmount;
                    db.SaveChanges();
                    transaction.Commit();

                    return RedirectToAction("Index");
                }
                catch
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "An error occurred while processing the purchase.");
                }
            }

            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "SupplierName", model.SupplierID);
            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class PurchaseViewModel
    {
        public int SupplierID { get; set; }
        public string MedicineName { get; set; }      // Medicine Name
        public int Quantity { get; set; }            // Quantity purchased
        public decimal UnitPrice { get; set; }       // Unit price
        public int CategoryID { get; set; }          // Category
        public string Manufacturer { get; set; }     // Manufacturer
        public DateTime? ExpiryDate { get; set; }    // Expiration date
    }

    public class PurchaseItemViewModel
    {
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Add these
        public int CategoryID { get; set; }
        public string Manufacturer { get; set; }
    }


    public class MultiplePurchaseViewModel
    {
        public int SupplierID { get; set; }
        public List<PurchaseItemViewModel> PurchaseItems { get; set; }
    }
}