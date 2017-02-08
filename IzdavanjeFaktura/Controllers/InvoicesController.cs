using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Security;
using System.Web.Mvc;
using IzdavanjeFaktura.Data;
using IzdavanjeFaktura.Models;
using IzdavanjeFaktura.Data.Repositories;
using IzdavanjeFaktura.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Rotativa.Options;

namespace IzdavanjeFaktura.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly InvoicesDbContext _context = new InvoicesDbContext();
        private readonly ApplicationUserManager _userManager;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceItemRepository _invoiceItemRepository;

        [Import(typeof(IVatCalculator))] private IVatCalculator _vatCalculator;

        public InvoicesController()
        {
            _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_context));
            _invoiceRepository = new InvoiceRepository(_context);
            _invoiceItemRepository = new InvoiceItemRepository(_context);
            MefLoader.Compose(this);
        }

        // GET: Invoices
        public ActionResult Index()
        {
            return View(_invoiceRepository.GetAll(GetCurrentUserId()));
        }

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

            if (invoiceToUpdate.Vat != invoice.Vat)
            {
                var fullprice = _vatCalculator.CalculateFullPrice(invoiceToUpdate.NoVatPrice, invoice.Vat);
                invoiceToUpdate.FullPrice = fullprice;
            }

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

        // GET: Invoices/Items/5
        public ActionResult Items(Guid id)
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
            var fullprice = _vatCalculator.CalculateFullPrice(invoice.NoVatPrice, invoice.Vat);
            invoice.FullPrice = fullprice;
            
            _invoiceRepository.Update(GetCurrentUserId(), invoice);

            _invoiceItemRepository.Add(item);

            return RedirectToAction("Items", new { id });
        }

        // POST: Invoices/RemoveItem/7
        [HttpPost, ActionName("RemoveItem")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItem(Guid id, Guid invoiceId)
        {
            var item = _invoiceItemRepository.Get(id);
            var invoice = _invoiceRepository.GetWithItems(GetCurrentUserId(), invoiceId);

            invoice.NoVatPrice -= item.TotalNoVatPrice;
            var fullprice = _vatCalculator.CalculateFullPrice(invoice.NoVatPrice, invoice.Vat);
            invoice.FullPrice = fullprice;

            _invoiceRepository.Update(GetCurrentUserId(), invoice);

            _invoiceItemRepository.Remove(GetCurrentUserId(), id);
            
            return RedirectToAction("Items", new { id = invoiceId });
        }

        // GET: Invoices/PrintInvoice/5
        public ActionResult PrintInvoice(Guid id)
        {
            var invoice = _invoiceRepository.GetWithItems(GetCurrentUserId(), id);

            var result = new Rotativa.ViewAsPdf("Invoice", invoice)
            {
                FileName = "invoice.pdf",
                PageSize = Size.A4
            };

            return result;
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