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
    public class MedicinesController : Controller
    {
        private PharmacyDBEntities db = new PharmacyDBEntities();

        // GET: Medicines
        public ActionResult Index()
        {
            var medicines = db.Medicines.Include(m => m.Category).ToList();
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View(medicines);
        }


        // GET: Medicines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medicine medicine = db.Medicines.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        // POST: Medicines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Medicine medicine)
        {
            var username = Session["Username"];
            if (ModelState.IsValid)
            {
                try
                {
                    medicine.CreatedBy = username.ToString();
                    db.Medicines.Add(medicine);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    // Get detailed validation errors
                    var errors = ex.EntityValidationErrors
                        .SelectMany(e => e.ValidationErrors)
                        .Select(e => e.PropertyName + ": " + e.ErrorMessage);

                    var fullErrorMessage = string.Join("; ", errors);

                    return Json(new { success = false, message = fullErrorMessage });
                }
            }

            var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
            return Json(new { success = false, message = error ?? "Error saving medicine" });
        }


        // POST: Medicines/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicine).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }

            var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
            return Json(new { success = false, message = error ?? "Error saving changes" });
        }

        // POST: Medicines/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            var medicine = db.Medicines.Find(id);
            if (medicine == null)
                return Json(new { success = false, message = "Medicine not found" });

            db.Medicines.Remove(medicine);
            db.SaveChanges();
            return Json(new { success = true });
        }

        // GET: Medicines/Edit/5
        public JsonResult Edit(int id)
        {
            var medicine = db.Medicines.Find(id);
            if (medicine == null)
                return Json(new { success = false, message = "Medicine not found" }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                success = true,
                MedicineID = medicine.MedicineID,
                Name = medicine.MedicineName,
                UnitPrice = medicine.UnitPrice,
                CategoryID = medicine.CategoryID
            }, JsonRequestBehavior.AllowGet);
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
