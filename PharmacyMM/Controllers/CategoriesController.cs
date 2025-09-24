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
    public class CategoriesController : Controller
    {
        private PharmacyDBEntities db = new PharmacyDBEntities();

        // GET: Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            var username = Session["Username"];
            if (ModelState.IsValid)
            {
                // Check for duplicate category name
                var existingCategory = db.Categories.FirstOrDefault(c => c.CategoryName == category.CategoryName);
                if (existingCategory != null)
                {
                    return Json(new { success = false, message = "A category with this name already exists." });
                }

                db.Categories.Add(category);
                try
                {
                    category.CreatedBy = username.ToString();
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error saving category: " + ex.Message });
                }
            }
            return Json(new { success = false, message = "Invalid data provided." });
        }
        // GET: Categories/Edit/5
        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
                return HttpNotFound();
            return View(category);
        }

        // POST: Categories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Categories.Find(category.CategoryID);
                if (existing == null)
                    return HttpNotFound();

                existing.CategoryName = category.CategoryName;
                existing.Description = category.Description;


                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //// GET: Categories/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    var category = db.Categories.Find(id);
        //    if (category == null)
        //        return HttpNotFound();
        //    return View(category);
        //}

        //// POST: Categories/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Category not found." });
            }

            // Check for related medicines
            var relatedMedicines = db.Medicines.Any(m => m.CategoryID == id);
            if (relatedMedicines)
            {
                return Json(new { success = false, message = "Cannot delete category because it is associated with one or more medicines." });
            }

            try
            {
                db.Categories.Remove(category);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting category: " + ex.Message });
            }
        }
    }
}