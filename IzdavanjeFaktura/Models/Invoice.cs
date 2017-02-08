using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IzdavanjeFaktura.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }
        [DisplayName("Invoice Number")]
        public string Number { get; set; }
        [DisplayName("Date Created")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
        [DisplayName("Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDateTime { get; set; }
        [DisplayName("No VAT Price")]
        [DataType(DataType.Currency)]
        public double NoVatPrice { get; set; }
        [DisplayName("VAT Country")]
        public VatType Vat { get; set; }
        [DisplayName("Full Price")]
        [DataType(DataType.Currency)]
        public double FullPrice { get; set; }
        [DisplayName("Bill To")]
        public string Receiver { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public Invoice(){ }

        public enum VatType
        {
            Croatia, Bosnia, Slovenia
        }
    }
}