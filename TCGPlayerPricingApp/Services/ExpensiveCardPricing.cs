using TCGPlayerPricingApp.Models;

namespace TCGPlayerPricingApp.Services
{
    public class ExpensiveCardPricing : IPricingStrategy
    {
        public bool CanApply(Card card)
        {
            return card.IsExpensive;
        }

        public double? CalculatePrice(Card card)
        {
            // For expensive cards, keep the original price
            return null; // null indicates "keep original price"
        }
    }
}
