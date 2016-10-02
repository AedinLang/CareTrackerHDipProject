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
            CareGiver careGiver = db.CareGivers.Include(c => c.Files).SingleOrDefault(c => c.ID == id);
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
        public ActionResult Create([Bind(Include = "ID,FirstName,Surname,AddressLine1,AddressLine2,Region,Email,PhoneNumber,Mobile,Qualifications,CV,References")] CareGiver careGiver, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var photoID = new File
                    {
                        FileName = System.IO.Path.GetFileName(upload.FileName),
                        FileType = FileType.PhotoID,
                        ContentType = upload.ContentType
                    };
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        photoID.Content = reader.ReadBytes(upload.ContentLength);
                    }
                    careGiver.Files = new List<File> { photoID };
                }
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
            CareGiver careGiver = db.CareGivers.Include(c => c.Files).SingleOrDefault(c => c.ID == id);
            if (careGiver == null)
            {
                return HttpNotFound();
            }

            var allClientsList = db.Clients.ToList();
            careGiver.AllClients = allClientsList.Select(o => new SelectListItem
            {
                Text = o.FirstName + " " + o.Surname,
                Value = o.ID.ToString()
            });
            PopulateCareGiverClientData(careGiver);
            return View(careGiver);
        }

        private void PopulateCareGiverClientData(CareGiver careGiver)
        {
            var allClients = db.Clients;
            var clients = new HashSet<int>(careGiver.Clients.Select(c => c.ID));
            var viewModel = new List<CareGiverClientData>();
            foreach (var client in allClients)
            {
                viewModel.Add(new CareGiverClientData
                {
                    ClientID = client.ID,
                    Selected = clients.Contains(client.ID)
                });
            }
            ViewBag.Clients = viewModel;
        }

        // POST: CareGiver/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection formCollection, string[] selectedClients, HttpPostedFileBase upload)
        {

            var careGiverToUpdate = db.CareGivers
                .Include(i => i.Clients)
                .Where(i => i.ID == id)
                .Single();

            if (TryUpdateModel(careGiverToUpdate, "",
      new string[] { "FirstName", "Surname", "AddressLine1", "AddressLine2", "Region", "Email", "PhoneNumber", "Mobile", "Qualifications", "CV", "References", "UserID" }))
            {
                /*try
                {
                    UpdateCareGiver(selectedClients, careGiverToUpdate);

                    db.Entry(careGiverToUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }*/
                try
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (careGiverToUpdate.Files.Any(f => f.FileType == FileType.PhotoID))
                        {
                            db.Files.Remove(careGiverToUpdate.Files.First(f => f.FileType == FileType.PhotoID));
                        }
                        var photoID = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.PhotoID,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            photoID.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        careGiverToUpdate.Files = new List<File> { photoID };
                    }

                    UpdateCareGiver(selectedClients, careGiverToUpdate);

                    db.Entry(careGiverToUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            PopulateCareGiverClientData(careGiverToUpdate);
            return View(careGiverToUpdate);
        }

        private void UpdateCareGiver(string[] selectedClients, CareGiver careGiverToUpdate)
        {
            if (selectedClients == null)
            {
                careGiverToUpdate.Clients = new List<Client>();
                return;
            }

            var selectedClientsHS = new HashSet<string>(selectedClients);
            var careGiverClients = new HashSet<int>(careGiverToUpdate.Clients.Select(t => t.ID));
            foreach (var client in db.Clients)
            {
                if (selectedClientsHS.Contains(client.ID.ToString()))
                {
                    if (!careGiverClients.Contains(client.ID))
                    {
                        careGiverToUpdate.Clients.Add(client);
                    }
                }
                else
                {
                    if (careGiverClients.Contains(client.ID))
                    {
                        careGiverToUpdate.Clients.Remove(client);
                    }
                }
            }
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
