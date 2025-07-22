using System;
using Xunit;
using TCGPlayerPricingApp.Models;
using TCGPlayerPricingApp.Services;

namespace TCGPlayerPricingApp.Tests
{
    public class PricingStrategyTests
    {
        [Fact]
        public void ExpensiveCardPricing_ShouldApplyToExpensiveCards()
        {
            // Arrange
            var strategy = new ExpensiveCardPricing();
            var expensiveCard = new Card { TcgMarketPrice = 50.0 };
            var cheapCard = new Card { TcgMarketPrice = 10.0 };
            
            // Act & Assert
            Assert.True(strategy.CanApply(expensiveCard));
            Assert.False(strategy.CanApply(cheapCard));
        }
        
        [Fact]
        public void ExpensiveCardPricing_ShouldReturnNull()
        {
            // Arrange
            var strategy = new ExpensiveCardPricing();
            var card = new Card { TcgMarketPrice = 50.0, TcgMarketplacePrice = 55.0 };
            
            // Act
            var result = strategy.CalculatePrice(card);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public void CheapCardPricing_ShouldApplyToCheapCards()
        {
            // Arrange
            var strategy = new CheapCardPricing();
            var cheapCard = new Card { TcgMarketPrice = 0.25 };
            var standardCard = new Card { TcgMarketPrice = 1.0 };
            
            // Act & Assert
            Assert.True(strategy.CanApply(cheapCard));
            Assert.False(strategy.CanApply(standardCard));
        }
        
        [Theory]
        [InlineData(0.25, 0.10, 0.50)] // Min price is 0.50
        [InlineData(0.25, 0.75, 0.75)] // TCG Low Price is higher
        public void CheapCardPricing_ShouldCalculateCorrectly(double marketPrice, double lowPrice, double expected)
        {
            // Arrange
            var strategy = new CheapCardPricing();
            var card = new Card { 
                TcgMarketPrice = marketPrice,
                TcgLowPrice = lowPrice
            };
            
            // Act
            var result = strategy.CalculatePrice(card);
            
            // Assert
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void StandardCardPricing_ShouldApplyToStandardCards()
        {
            // Arrange
            var strategy = new StandardCardPricing();
            var cheapCard = new Card { TcgMarketPrice = 0.25 };
            var standardCard = new Card { TcgMarketPrice = 15.0 };
            var expensiveCard = new Card { TcgMarketPrice = 35.0 };
            
            // Act & Assert
            Assert.False(strategy.CanApply(cheapCard));
            Assert.True(strategy.CanApply(standardCard));
            Assert.False(strategy.CanApply(expensiveCard));
        }
        
        [Theory]
        [InlineData(15.0, 10.0, 16.0, 15.5)] // Average of market and low with shipping
        [InlineData(15.0, 16.0, 16.0, 16.0)] // TCG Low Price is higher than average
        [InlineData(0.40, 0.10, 0.20, 0.50)] // Min price is 0.50
        public void StandardCardPricing_ShouldCalculateCorrectly(
            double marketPrice, double lowPrice, double lowWithShipping, double expected)
        {
            // Arrange
            var strategy = new StandardCardPricing();
            var card = new Card { 
                TcgMarketPrice = marketPrice,
                TcgLowPrice = lowPrice,
                TcgLowWithShipping = lowWithShipping
            };
            
            // Act
            var result = strategy.CalculatePrice(card);
            
            // Assert
            Assert.Equal(expected, result);
        }
    }
}
