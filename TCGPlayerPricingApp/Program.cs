using System;
using TCGPlayerPricingApp.Data;
using TCGPlayerPricingApp.Services;

namespace TCGPlayerPricingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputPath = "../input.csv";
            var outputPath = "../cleaned_output.csv";
            
            Console.WriteLine("TCG Player Pricing App");
            Console.WriteLine("=====================");
            
            // Create repository and services
            ICsvRepository repository = new CsvRepository();
            var pricingService = new PricingService();
            
            // Read cards from CSV
            Console.WriteLine($"Reading cards from {inputPath}...");
            var (headers, cards) = repository.ReadCards(inputPath);
            Console.WriteLine($"Found {cards.Count} cards with quantity > 0");
            
            // Calculate prices
            Console.WriteLine("Calculating prices...");
            pricingService.CalculatePrices(cards);
            
            // Write updated cards to CSV
            Console.WriteLine($"Writing results to {outputPath}...");
            repository.WriteCards(outputPath, headers, cards);
            
            Console.WriteLine($"Processing complete. {cards.Count} cards processed.");
            Console.WriteLine("TCG Marketplace Price has been replaced with Estimated Price where applicable.");
        }
    }
}
