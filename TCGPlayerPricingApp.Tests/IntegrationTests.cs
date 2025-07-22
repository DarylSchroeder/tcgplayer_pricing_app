using System;
using System.IO;
using System.Linq;
using Xunit;
using TCGPlayerPricingApp.Data;
using TCGPlayerPricingApp.Services;

namespace TCGPlayerPricingApp.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void ProcessingPipeline_ShouldWorkEndToEnd()
        {
            // Arrange
            var testInputPath = Path.GetTempFileName();
            var testOutputPath = Path.GetTempFileName();
            
            try
            {
                // Create test input file
                File.WriteAllText(testInputPath, @"Product Name,TCG Market Price,TCG Low Price,TCG Low Price With Shipping,TCG Marketplace Price,Total Quantity
""Expensive Card"",50.00,45.00,48.00,55.00,1
""Cheap Card"",0.25,0.20,1.50,0.30,1
""Standard Card"",15.00,12.00,14.00,16.00,1
""Zero Quantity Card"",10.00,8.00,9.00,11.00,0");
                
                // Create repository and service
                var repository = new CsvRepository();
                var pricingService = new PricingService();
                
                // Act
                var (headers, cards) = repository.ReadCards(testInputPath);
                pricingService.CalculatePrices(cards);
                repository.WriteCards(testOutputPath, headers, cards);
                
                // Read the output file
                var outputLines = File.ReadAllLines(testOutputPath);
                
                // Assert
                Assert.Equal(4, outputLines.Length); // Header + 3 cards (zero quantity card excluded)
                
                // Check that the expensive card price is unchanged
                Assert.Contains("\"Expensive Card\"", outputLines[1]);
                Assert.Contains("55", outputLines[1]); // Original price preserved
                
                // Check that the cheap card price is 0.50
                Assert.Contains("\"Cheap Card\"", outputLines[2]);
                Assert.Contains("0.50", outputLines[2]); // Min price applied
                
                // Check that the standard card price is calculated correctly
                Assert.Contains("\"Standard Card\"", outputLines[3]);
                Assert.Contains("14.50", outputLines[3]); // Average of market and low with shipping
            }
            finally
            {
                // Clean up
                if (File.Exists(testInputPath)) File.Delete(testInputPath);
                if (File.Exists(testOutputPath)) File.Delete(testOutputPath);
            }
        }
    }
}
