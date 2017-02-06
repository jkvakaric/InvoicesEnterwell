using System;
using System.Collections.Generic;
using IzdavanjeFaktura.Models;

namespace IzdavanjeFaktura.Data.Repositories
{
    public interface IInvoiceRepository
    {
        IEnumerable<Invoice> GetAll(Guid userId);
        Invoice Get(Guid userId, Guid invoiceId);
        void Add(Invoice invoice);
        bool Update(Guid userId, Invoice invoice);
        bool Remove(Guid userId, Guid invoiceId);
        Invoice GetWithItems(Guid userId, Guid invoiceId);
    }
}