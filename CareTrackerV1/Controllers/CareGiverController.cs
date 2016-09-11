using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CareTrackerV1.Models;
using CareTrackerV1.ViewModel;

namespace CareTrackerV1.Controllers
{
    public class CareGiverController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CareGiver
        public ActionResult Index(int? id, int? clientID)
        {
            var viewModel = new CareGiverIndexData();

            viewModel.CareGivers = db.CareGivers
                .Include(i => i.Clients)
                .Include(i => i.Visits)
                .OrderBy(i => i.Surname);

            

            if (id != null)
            {
                ViewBag.CareGiverID = id.Value;
                viewModel.Clients = viewModel.CareGivers.
                    Where(i => i.ID == id.Value).Single().Clients;

            }
            
            if (clientID != null)
            {
                ViewBag.ClientID = clientID.Value;

                viewModel.Visits = from v in db.Visits
                                   where
                                   (v.ClientID == clientID)
                                   where
                                   (v.CareGiverID == id.Value)
                                   select v;
           }

            return View(viewModel);

            }

        public ActionResult NonAdminIndex(int? id, int? clientID)
        {

            var viewModel = new CareGiverIndexData();

            viewModel.Clients = db.Clients
                .Include(i => i.CareGivers)
                .Include(i => i.Visits);

            if (id != null)
            {
                ViewBag.ClientID = clientID.Value;
                viewModel.Clients = viewModel.CareGivers.
                    Where(i => i.ID == id.Value).Single().Clients;
            }

            if (clientID != null)
            {
                ViewBag.ClientID = clientID.Value;

                viewModel.Visits = from v in db.Visits
                                   where
                                   (v.ClientID == clientID)
                                   where
                                   (v.CareGiverID == id.Value)
                                   select v;
            }

            /*if (clientID != null)
            {
                ViewBag.ClientID = clientID.Value;
                viewModel.Visits = viewModel.Clients.
                    Where(i => i.ID == clientID.Value).Single().Visits;
            }*/

            return View(viewModel);

        }

        // GET: CareGiver/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CareGiver careGiver = db.CareGivers.Find(id);
            if (careGiver == null)
            {
                return HttpNotFound();
            }
            return View(careGiver);
        }

        // GET: CareGiver/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CareGiver/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,Surname,AddressLine1,AddressLine2,Region,Email,PhoneNumber,Mobile,Qualifications,CV,References")] CareGiver careGiver)
        {
            if (ModelState.IsValid)
            {
                db.CareGivers.Add(careGiver);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(careGiver);
        }

        // GET: CareGiver/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CareGiver careGiver = db.CareGivers.Find(id);
            if (careGiver == null)
            {
                return HttpNotFound();
            }
            return View(careGiver);
        }

        // POST: CareGiver/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,Surname,AddressLine1,AddressLine2,Region,Email,PhoneNumber,Mobile,Qualifications,CV,References")] CareGiver careGiver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(careGiver).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(careGiver);
        }

        // GET: CareGiver/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CareGiver careGiver = db.CareGivers.Find(id);
            if (careGiver == null)
            {
                return HttpNotFound();
            }
            return View(careGiver);
        }

        // POST: CareGiver/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CareGiver careGiver = db.CareGivers.Find(id);
            db.CareGivers.Remove(careGiver);
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
