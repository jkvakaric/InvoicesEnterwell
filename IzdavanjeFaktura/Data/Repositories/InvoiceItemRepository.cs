using System;
using System.Collections.Generic;
using System.Linq;
using IzdavanjeFaktura.Models;

namespace IzdavanjeFaktura.Data.Repositories
{
    public class InvoiceItemRepository : IInvoiceItemRepository
    {
        private readonly InvoicesDbContext _context;

        public InvoiceItemRepository(InvoicesDbContext context)
        {
            _context = context;
        }

        public InvoiceItem Get(Guid itemId)
        {
            var item = _context.InvoiceItems
                .FirstOrDefault(ii => ii.Id == itemId);
            return item;
        }

        public void Add(InvoiceItem item)
        {
            _context.InvoiceItems.Add(item);
            _context.SaveChanges();
        }

        public bool Remove(Guid userId, Guid itemId)
        {
            var item = Get(itemId);
            if (item == null) return false;
            if (item.Invoice.Creator.Id != userId.ToString())
            {
                return false;
            }

            _context.InvoiceItems.Remove(item);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<InvoiceItem> GetInvoiceItems(Guid invoiceId)
        {
            var invoiceItems = _context.InvoiceItems
                .Where(ii => ii.Invoice.Id == invoiceId);

            return invoiceItems;
        }
    }
}