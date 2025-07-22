using System;
using TCGPlayerPricingApp.Models;

namespace TCGPlayerPricingApp.Services
{
    public class StandardCardPricing : IPricingStrategy
    {
        private const double MinimumPrice = 0.50;
        private const double CheapThreshold = 0.30;
        private const double ExpensiveThreshold = 30.0;

        public bool CanApply(Card card)
        {
            return card.TcgMarketPrice > CheapThreshold && card.TcgMarketPrice <= ExpensiveThreshold;
        }

        public double? CalculatePrice(Card card)
        {
            // For standard cards: max($0.50, max(TCG Low Price, average of TCG Low Price With Shipping and TCG Market Price))
            double avg = (card.TcgLowWithShipping + card.TcgMarketPrice) / 2;
            return Math.Round(Math.Max(MinimumPrice, Math.Max(card.TcgLowPrice, avg)), 2);
        }
    }
}
