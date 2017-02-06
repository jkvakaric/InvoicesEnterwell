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

        public void Remove(Guid userId, Guid itemId)
        {
            var item = Get(itemId);
            if (item == null) return;
            if (item.Invoice.Creator.Id != userId.ToString())
            {
                throw new UnauthorizedAccessException();
            }

            _context.InvoiceItems.Remove(item);
            _context.SaveChanges();
        }

        public IEnumerable<InvoiceItem> GetInvoiceItems(Guid userId, Guid invoiceId)
        {
            var invoiceItems = _context.InvoiceItems
                .Where(ii => ii.Invoice.Id == invoiceId);

            if (!invoiceItems.Any()) return invoiceItems;
            if (invoiceItems.First().Invoice.Creator.Id != userId.ToString())
            {
                throw new UnauthorizedAccessException();
            }

            return invoiceItems;
        }
    }
}