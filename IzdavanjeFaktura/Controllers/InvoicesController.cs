using System;
using System.Net;
using System.Security;
using System.Web.Mvc;
using IzdavanjeFaktura.Data;
using IzdavanjeFaktura.Models;
using IzdavanjeFaktura.Data.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IzdavanjeFaktura.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceItemRepository _invoiceItemRepository;

        private readonly InvoicesDbContext _context = new InvoicesDbContext();

        public InvoicesController()
        {
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
            _invoiceRepository = new InvoiceRepository(_context);
            _invoiceItemRepository = new InvoiceItemRepository(_context);
        }

        public InvoicesController(IInvoiceRepository invoiceRepository, IInvoiceItemRepository invoiceItemRepository)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceItemRepository = invoiceItemRepository;
        }

        // GET: Invoices
        public ActionResult Index()
        {
            return View(_invoiceRepository.GetAll(GetCurrentUserId()));
        }

        // GET: Invoices/Details/5
        //public ActionResult Details(Guid? id)
        //{
        //    if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    var invoice = _invoiceRepository.Get(GetCurrentUserId(), id.Value);
        //    if (invoice == null) return HttpNotFound();
            
        //    return View(invoice);
        //}

        // GET: Invoices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Number,DueDateTime,Vat,Receiver")] Invoice invoice)
        {
            if (!ModelState.IsValid) return View(invoice);
            var user = _userManager.FindById(User.Identity.GetUserId());

            invoice.Id = Guid.NewGuid();
            invoice.DateCreated = DateTime.Now;
            invoice.Creator = user;

            _invoiceRepository.Add(invoice);

            return RedirectToAction("Index");
        }

        // GET: Invoices/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var invoice = _invoiceRepository.Get(GetCurrentUserId(), id.Value);
            if (invoice == null) return HttpNotFound();

            return View(invoice);
        }

        // POST: Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Number,DueDateTime,Vat,Receiver")] Invoice invoice)
        {
            if (!ModelState.IsValid) return View(invoice);

            var invoiceToUpdate = _invoiceRepository.Get(GetCurrentUserId(), invoice.Id);
            invoiceToUpdate.Number = invoice.Number;
            invoiceToUpdate.DueDateTime = invoice.DueDateTime;
            invoiceToUpdate.Vat = invoice.Vat;
            invoiceToUpdate.Receiver = invoice.Receiver;

            _invoiceRepository.Update(GetCurrentUserId(), invoiceToUpdate);

            return RedirectToAction("Index");
        }

        // GET: Invoices/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var invoice = _invoiceRepository.Get(GetCurrentUserId(), id.Value);
            if (invoice == null) return HttpNotFound();

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            _invoiceRepository.Remove(GetCurrentUserId(), id);

            return RedirectToAction("Index");
        }

        // GET: Invoices/GetItems/5
        public ActionResult GetItems(Guid id)
        {
            var invoiceItems = _invoiceItemRepository.GetInvoiceItems(GetCurrentUserId(), id);
            if (invoiceItems == null) return HttpNotFound();

            var invoice = _invoiceRepository.Get(GetCurrentUserId(), id);
            ViewData.Add("invoiceNumber", invoice.Number);
            ViewData.Add("invoiceId", id);

            return View(invoiceItems);
        }

        // GET Invoices/AddItem/5
        public ActionResult AddItem(Guid id)
        {
            var invoice = _invoiceRepository.Get(GetCurrentUserId(), id);

            ViewData.Add("invoiceId", id);
            ViewData.Add("invoiceNumber", invoice.Number);

            return View();
        }

        // POST: Invoices/AddItem/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem([Bind(Include = "Description,NoVatItemPrice,Quantity")] InvoiceItem item, Guid id)
        {
            if (!ModelState.IsValid) return View(item);
            var invoice = _invoiceRepository.GetWithItems(GetCurrentUserId(), id);

            item.Id = Guid.NewGuid();
            item.TotalNoVatPrice = item.NoVatItemPrice * item.Quantity;
            item.Invoice = invoice;

            invoice.NoVatPrice += item.TotalNoVatPrice;






            _invoiceRepository.Update(GetCurrentUserId(), invoice);

            _invoiceItemRepository.Add(item);

            return RedirectToAction("GetItems", new { id });
        }

        // POST: Invoices/RemoveItem/5
        [HttpPost, ActionName("RemoveItem")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItem(Guid id, Guid invoiceId)
        {
            var item = _invoiceItemRepository.Get(id);
            var invoice = _invoiceRepository.GetWithItems(GetCurrentUserId(), invoiceId);

            invoice.NoVatPrice -= item.TotalNoVatPrice;






            _invoiceRepository.Update(GetCurrentUserId(), invoice);

            _invoiceItemRepository.Remove(GetCurrentUserId(), id);

            return RedirectToAction("GetItems", new { id = invoiceId });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private Guid GetCurrentUserId()
        {
            Guid userId;
            if (!Guid.TryParse(User.Identity.GetUserId(), out userId))
            {
                throw new SecurityException();
            }
            return userId;
        }
    }
}
