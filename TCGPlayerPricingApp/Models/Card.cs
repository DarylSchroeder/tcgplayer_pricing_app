using System;

namespace TCGPlayerPricingApp.Models
{
    public class Card
    {
        public string ProductName { get; set; }
        public double TcgMarketPrice { get; set; }
        public double TcgLowPrice { get; set; }
        public double TcgLowWithShipping { get; set; }
        public double TcgMarketplacePrice { get; set; }
        public int TotalQuantity { get; set; }
        
        // This will be calculated by pricing strategies
        public double? EstimatedPrice { get; set; }
        
        public bool IsExpensive => TcgMarketPrice > 30 || TcgMarketplacePrice > 30;
    }
}
