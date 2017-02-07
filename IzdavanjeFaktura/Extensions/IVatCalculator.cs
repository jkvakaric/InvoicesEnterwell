using IzdavanjeFaktura.Models;

namespace IzdavanjeFaktura.Extensions
{
    public interface IVatCalculator
    {
        double CalculateFullPrice(double noVatPrice, Invoice.VatType type);
    }
}
