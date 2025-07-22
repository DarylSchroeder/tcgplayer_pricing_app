using TCGPlayerPricingApp.Models;

namespace TCGPlayerPricingApp.Services
{
    public interface IPricingStrategy
    {
        bool CanApply(Card card);
        double? CalculatePrice(Card card);
    }
}
