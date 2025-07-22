using System;
using TCGPlayerPricingApp.Models;

namespace TCGPlayerPricingApp.Services
{
    public class CheapCardPricing : IPricingStrategy
    {
        private const double MinimumPrice = 0.50;
        private const double CheapThreshold = 0.30;

        public bool CanApply(Card card)
        {
            return card.TcgMarketPrice <= CheapThreshold;
        }

        public double? CalculatePrice(Card card)
        {
            // For very cheap cards: max($0.50, TCG Low Price)
            return Math.Round(Math.Max(MinimumPrice, card.TcgLowPrice), 2);
        }
    }
}
