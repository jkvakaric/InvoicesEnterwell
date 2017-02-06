using System.Data.Entity;
using IzdavanjeFaktura.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IzdavanjeFaktura.Data
{
    public class InvoicesDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        public InvoicesDbContext() : base("DefaultConnection"){ }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>().HasKey(i => i.Id);
            modelBuilder.Entity<Invoice>().Property(i => i.Number).IsRequired();
            modelBuilder.Entity<Invoice>().Property(i => i.DateCreated).IsRequired();
            modelBuilder.Entity<Invoice>().Property(i => i.DueDateTime).IsRequired();
            modelBuilder.Entity<Invoice>().Property(i => i.NoVatPrice).IsRequired();
            modelBuilder.Entity<Invoice>().Property(i => i.FullPrice).IsRequired();
            modelBuilder.Entity<Invoice>().Property(i => i.Vat).IsRequired();
            modelBuilder.Entity<Invoice>().HasMany(i => i.InvoiceItems).WithRequired(ii => ii.Invoice);
            modelBuilder.Entity<Invoice>().HasRequired(i => i.Creator).WithMany();

            modelBuilder.Entity<InvoiceItem>().HasKey(ii => ii.Id);
            modelBuilder.Entity<InvoiceItem>().Property(ii => ii.Description).IsRequired();
            modelBuilder.Entity<InvoiceItem>().Property(ii => ii.NoVatItemPrice).IsRequired();
            modelBuilder.Entity<InvoiceItem>().Property(ii => ii.Quantity).IsRequired();
            modelBuilder.Entity<InvoiceItem>().Property(ii => ii.TotalNoVatPrice).IsRequired();
        }

        public static InvoicesDbContext Create()
        {
            return new InvoicesDbContext();
        }
    }
}