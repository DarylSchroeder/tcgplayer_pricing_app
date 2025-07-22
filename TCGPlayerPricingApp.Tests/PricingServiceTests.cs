using System.Collections.Generic;
using Xunit;
using TCGPlayerPricingApp.Models;
using TCGPlayerPricingApp.Services;

namespace TCGPlayerPricingApp.Tests
{
    public class PricingServiceTests
    {
        [Fact]
        public void CalculatePrices_ShouldApplyCorrectStrategies()
        {
            // Arrange
            var service = new PricingService();
            var cards = new List<Card>
            {
                // Expensive card
                new Card { 
                    ProductName = "Expensive Card", 
                    TcgMarketPrice = 50.0, 
                    TcgMarketplacePrice = 55.0,
                    TcgLowPrice = 45.0,
                    TcgLowWithShipping = 48.0
                },
                
                // Cheap card
                new Card { 
                    ProductName = "Cheap Card", 
                    TcgMarketPrice = 0.25, 
                    TcgMarketplacePrice = 0.30,
                    TcgLowPrice = 0.20,
                    TcgLowWithShipping = 1.50
                },
                
                // Standard card
                new Card { 
                    ProductName = "Standard Card", 
                    TcgMarketPrice = 15.0, 
                    TcgMarketplacePrice = 16.0,
                    TcgLowPrice = 12.0,
                    TcgLowWithShipping = 14.0
                }
            };
            
            // Act
            service.CalculatePrices(cards);
            
            // Assert
            Assert.Null(cards[0].EstimatedPrice); // Expensive card - keep original price
            Assert.Equal(0.50, cards[1].EstimatedPrice); // Cheap card - max(0.50, 0.20)
            Assert.Equal(14.50, cards[2].EstimatedPrice); // Standard card - avg(14.0, 15.0)
        }
    }
}
