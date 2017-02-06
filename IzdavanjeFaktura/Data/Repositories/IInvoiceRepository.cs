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
        void Update(Guid userId, Invoice invoice);
        void Remove(Guid userId, Guid invoiceId);
        Invoice GetWithItems(Guid userId, Guid invoiceId);
    }
}