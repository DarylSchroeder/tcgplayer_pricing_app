using System.Collections.Generic;
using TCGPlayerPricingApp.Models;

namespace TCGPlayerPricingApp.Services
{
    public class PricingService
    {
        private readonly List<IPricingStrategy> _strategies;

        public PricingService()
        {
            // Register strategies in order of evaluation
            _strategies = new List<IPricingStrategy>
            {
                new ExpensiveCardPricing(),
                new CheapCardPricing(),
                new StandardCardPricing()
            };
        }

        public void CalculatePrices(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                foreach (var strategy in _strategies)
                {
                    if (strategy.CanApply(card))
                    {
                        card.EstimatedPrice = strategy.CalculatePrice(card);
                        break;
                    }
                }
            }
        }
    }
}
