using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Card
{
    public string ProductName;
    public double TcgMarketPrice;
    public double TcgLowPrice;
    public double TcgLowWithShipping;
    public double TcgMarketplacePrice;
    public int TotalQuantity;

    public bool IsExpensive => TcgMarketPrice > 30 || TcgMarketplacePrice > 30;

    public double? EstimatedPrice
    {
        get
        {
            if (IsExpensive) return null;
            if (TcgMarketPrice <= 0.30)
                return Math.Round(Math.Max(0.50, TcgLowPrice), 2);

            double avg = (TcgLowWithShipping + TcgMarketPrice) / 2;
            return Math.Round(Math.Max(0.50, Math.Max(TcgLowPrice, avg)), 2);
        }
    }

    public static Card FromCsv(string[] headers, string[] row)
    {
        double Parse(string header) =>
            double.TryParse(row[Array.IndexOf(headers, header)], NumberStyles.Any, CultureInfo.InvariantCulture, out double val) ? val : 0.0;
        int ParseInt(string header) =>
            int.TryParse(row[Array.IndexOf(headers, header)], out int val) ? val : 0;
        
        return new Card
        {
            ProductName = row[Array.IndexOf(headers, "Product Name")],
            TcgMarketPrice = Parse("TCG Market Price"),
            TcgLowPrice = Parse("TCG Low Price"),
            TcgLowWithShipping = Parse("TCG Low Price With Shipping"),
            TcgMarketplacePrice = Parse("TCG Marketplace Price"),
            TotalQuantity = ParseInt("Total Quantity")
        };
    }

    public string ToCsv(string[] baseHeaders)
    {
        return string.Join(",",
            baseHeaders.Select(h => h == "Estimated Price"
                ? EstimatedPrice?.ToString("0.00", CultureInfo.InvariantCulture) ?? ""
                : h == "Is Expensive"
                    ? IsExpensive.ToString()
                    : h == "Product Name"
                        ? "\"" + ProductName.Replace("\"", "\"\"") + "\""
                        : h == "TCG Market Price" ? TcgMarketPrice.ToString(CultureInfo.InvariantCulture)
                        : h == "TCG Low Price" ? TcgLowPrice.ToString(CultureInfo.InvariantCulture)
                        : h == "TCG Low Price With Shipping" ? TcgLowWithShipping.ToString(CultureInfo.InvariantCulture)
                        : h == "TCG Marketplace Price" ? TcgMarketplacePrice.ToString(CultureInfo.InvariantCulture)
                        : h == "Total Quantity" ? TotalQuantity.ToString()
                        : "")
        );
    }
}

class Program
{
    static void Main()
    {
        var inputPath = "input.csv"; // Replace with your actual input file path
        var outputPath = "cleaned_output.csv";

        var lines = File.ReadAllLines(inputPath);
        var headers = lines[0].Split(',');

        var baseHeaders = headers.ToList();
        if (!baseHeaders.Contains("Is Expensive")) baseHeaders.Add("Is Expensive");
        if (!baseHeaders.Contains("Estimated Price")) baseHeaders.Add("Estimated Price");

        var cleaned = lines
            .Skip(1)
            .Select(line => line.Split(','))
            .Select(row => Card.FromCsv(headers, row))
            .Where(card => card.TotalQuantity > 0)
            .ToList();

        var outputLines = new List<string> { string.Join(",", baseHeaders) };
        outputLines.AddRange(cleaned.Select(card => card.ToCsv(baseHeaders.ToArray())));

        File.WriteAllLines(outputPath, outputLines);

        Console.WriteLine($"Processed {cleaned.Count} records. Output written to {outputPath}.");
    }
}
