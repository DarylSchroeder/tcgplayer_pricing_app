using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TCGPlayerPricingApp.Models;
using TCGPlayerPricingApp.Utilities;

namespace TCGPlayerPricingApp.Data
{
    public class CsvRepository : ICsvRepository
    {
        public (string[] Headers, List<Card> Cards) ReadCards(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var headers = CsvParser.ParseLine(lines[0]);
            var cards = new List<Card>();
            
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var row = CsvParser.ParseLine(lines[i]);
                    if (row.Length != headers.Length)
                    {
                        Console.WriteLine($"Skipping row {i+1}: Column count mismatch (expected {headers.Length}, got {row.Length})");
                        continue;
                    }
                    
                    var card = CreateCardFromRow(headers, row);
                    if (card.TotalQuantity > 0)
                    {
                        cards.Add(card);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing row {i+1}: {ex.Message}");
                }
            }
            
            return (headers, cards);
        }
        
        public void WriteCards(string filePath, string[] headers, IEnumerable<Card> cards)
        {
            var outputLines = new List<string> { string.Join(",", headers) };
            outputLines.AddRange(cards.Select(card => CardToCsvLine(card, headers)));
            
            File.WriteAllLines(filePath, outputLines);
        }
        
        private Card CreateCardFromRow(string[] headers, string[] row)
        {
            int GetIndex(string columnName) => Array.IndexOf(headers, columnName);
            
            return new Card
            {
                ProductName = row[GetIndex("Product Name")],
                TcgMarketPrice = CsvParser.ParseDouble(row[GetIndex("TCG Market Price")]),
                TcgLowPrice = CsvParser.ParseDouble(row[GetIndex("TCG Low Price")]),
                TcgLowWithShipping = CsvParser.ParseDouble(row[GetIndex("TCG Low Price With Shipping")]),
                TcgMarketplacePrice = CsvParser.ParseDouble(row[GetIndex("TCG Marketplace Price")]),
                TotalQuantity = CsvParser.ParseInt(row[GetIndex("Total Quantity")])
            };
        }
        
        private string CardToCsvLine(Card card, string[] headers)
        {
            var values = new Dictionary<string, string>();
            
            foreach (var header in headers)
            {
                switch (header)
                {
                    case "Product Name":
                        values[header] = "\"" + card.ProductName.Replace("\"", "\"\"") + "\"";
                        break;
                    case "TCG Market Price":
                        values[header] = card.TcgMarketPrice.ToString(CultureInfo.InvariantCulture);
                        break;
                    case "TCG Low Price":
                        values[header] = card.TcgLowPrice.ToString(CultureInfo.InvariantCulture);
                        break;
                    case "TCG Low Price With Shipping":
                        values[header] = card.TcgLowWithShipping.ToString(CultureInfo.InvariantCulture);
                        break;
                    case "TCG Marketplace Price":
                        // Replace TCG Marketplace Price with Estimated Price
                        values[header] = card.EstimatedPrice?.ToString("0.00", CultureInfo.InvariantCulture) ?? 
                                         card.TcgMarketplacePrice.ToString(CultureInfo.InvariantCulture);
                        break;
                    case "Total Quantity":
                        values[header] = card.TotalQuantity.ToString();
                        break;
                    default:
                        values[header] = "";
                        break;
                }
            }
            
            return string.Join(",", headers.Select(h => values.ContainsKey(h) ? values[h] : ""));
        }
    }
}
