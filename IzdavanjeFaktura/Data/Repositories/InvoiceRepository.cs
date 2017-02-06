using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using IzdavanjeFaktura.Models;

namespace IzdavanjeFaktura.Data.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoicesDbContext _context;

        public InvoiceRepository(InvoicesDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Invoice> GetAll(Guid userId)
        {
            return _context.Invoices.Where(i => i.Creator.Id == userId.ToString()).ToList();
        }

        public Invoice Get(Guid userId, Guid invoiceId)
        {
            var invoice = _context.Invoices
                .FirstOrDefault(i => i.Id == invoiceId);

            if (invoice == null) return null;
            return invoice.Creator.Id != userId.ToString() ? null : invoice;
        }

        public void Add(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
        }

        public bool Update(Guid userId, Invoice invoice)
        {
            if (invoice.Creator.Id != userId.ToString()) return false;


            _context.Entry(invoice).State = EntityState.Modified;
            _context.SaveChanges();
            return true;
        }

        public bool Remove(Guid userId, Guid invoiceId)
        {
            var invoice = Get(userId, invoiceId);
            if (invoice == null) return false;

            _context.Invoices.Remove(invoice);
            _context.SaveChanges();
            return true;
        }

        public Invoice GetWithItems(Guid userId, Guid invoiceId)
        {
            var invoice = _context.Invoices.Include(i => i.InvoiceItems)
                .FirstOrDefault(i => i.Id == invoiceId);

            if (invoice == null) return null;
            return invoice.Creator.Id != userId.ToString() ? null : invoice;
        }
    }
}