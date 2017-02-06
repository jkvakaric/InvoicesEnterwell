using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IzdavanjeFaktura.Models
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Currency)]
        [DisplayName("NoVAT Item Price")]
        public double NoVatItemPrice { get; set; }
        public int Quantity { get; set; }
        [DataType(DataType.Currency)]
        [DisplayName("Total NoVAT Price")]
        public double TotalNoVatPrice { get; set; }

        public virtual Invoice Invoice { get; set; }

        public InvoiceItem() { }
    }
}