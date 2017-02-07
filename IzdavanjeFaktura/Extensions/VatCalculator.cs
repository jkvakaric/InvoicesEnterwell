using System;
using System.ComponentModel.Composition;
using IzdavanjeFaktura.Models;

namespace IzdavanjeFaktura.Extensions
{
    [Export(typeof(IVatCalculator))]
    public class VatCalculator : IVatCalculator
    {
        public double CalculateFullPrice(double noVatPrice, Invoice.VatType type)
        {
            switch (type)
            {
                case Invoice.VatType.Croatian:
                    return Calculate(noVatPrice, 0.25);
                case Invoice.VatType.Bosnian:
                    return Calculate(noVatPrice, 0.17);
                case Invoice.VatType.Slovenian:
                    return Calculate(noVatPrice, 0.22);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static double Calculate(double noVat, double percentage)
        {
            return noVat / (1 - percentage);
        }
    }
}