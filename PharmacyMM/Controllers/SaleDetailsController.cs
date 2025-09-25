using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PharmacyMM.Models;

namespace PharmacyMM.Controllers
{
    public class SaleDetailsController : Controller
    {
        private PharmacyDBEntities db = new PharmacyDBEntities();

        // GET: SaleDetails
        public ActionResult Index()
        {
            var saleDetails = db.SaleDetails.Include(s => s.Medicine).Include(s => s.Sale);
            return View(saleDetails.ToList());
        }

        // GET: SaleDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleDetail saleDetail = db.SaleDetails.Find(id);
            if (saleDetail == null)
            {
                return HttpNotFound();
            }
            return View(saleDetail);
        }

        // GET: SaleDetails/Create
        public ActionResult Create()
        {
            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName");
            ViewBag.SaleID = new SelectList(db.Sales, "SaleID", "SaleID");
            return View();
        }

        // POST: SaleDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SaleDetailID,SaleID,MedicineID,Quantity,UnitPrice,SubTotal")] SaleDetail saleDetail)
        {
            if (ModelState.IsValid)
            {
                db.SaleDetails.Add(saleDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName", saleDetail.MedicineID);
            ViewBag.SaleID = new SelectList(db.Sales, "SaleID", "SaleID", saleDetail.SaleID);
            return View(saleDetail);
        }

        // GET: SaleDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleDetail saleDetail = db.SaleDetails.Find(id);
            if (saleDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName", saleDetail.MedicineID);
            ViewBag.SaleID = new SelectList(db.Sales, "SaleID", "SaleID", saleDetail.SaleID);
            return View(saleDetail);
        }

        // POST: SaleDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SaleDetailID,SaleID,MedicineID,Quantity,UnitPrice,SubTotal")] SaleDetail saleDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(saleDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MedicineID = new SelectList(db.Medicines, "MedicineID", "MedicineName", saleDetail.MedicineID);
            ViewBag.SaleID = new SelectList(db.Sales, "SaleID", "SaleID", saleDetail.SaleID);
            return View(saleDetail);
        }

        // GET: SaleDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleDetail saleDetail = db.SaleDetails.Find(id);
            if (saleDetail == null)
            {
                return HttpNotFound();
            }
            return View(saleDetail);
        }

        // POST: SaleDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SaleDetail saleDetail = db.SaleDetails.Find(id);
            db.SaleDetails.Remove(saleDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
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
}
