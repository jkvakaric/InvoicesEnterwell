using System;
using System.Collections.Generic;
using IzdavanjeFaktura.Models;

namespace IzdavanjeFaktura.Data.Repositories
{
    public interface IInvoiceItemRepository
    {
        InvoiceItem Get(Guid itemId);
        void Add(InvoiceItem item);
        void Remove(Guid userId, Guid itemId);
        IEnumerable<InvoiceItem> GetInvoiceItems(Guid invoiceId, Guid userId);
    }
}
