using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

    public string ToCsv(string[] headers)
    {
        var values = new Dictionary<string, string>();
        
        // Fill in all original values
        foreach (var header in headers)
        {
            switch (header)
            {
                case "Product Name":
                    values[header] = "\"" + ProductName.Replace("\"", "\"\"") + "\"";
                    break;
                case "TCG Market Price":
                    values[header] = TcgMarketPrice.ToString(CultureInfo.InvariantCulture);
                    break;
                case "TCG Low Price":
                    values[header] = TcgLowPrice.ToString(CultureInfo.InvariantCulture);
                    break;
                case "TCG Low Price With Shipping":
                    values[header] = TcgLowWithShipping.ToString(CultureInfo.InvariantCulture);
                    break;
                case "TCG Marketplace Price":
                    // Replace TCG Marketplace Price with Estimated Price
                    values[header] = EstimatedPrice?.ToString("0.00", CultureInfo.InvariantCulture) ?? TcgMarketplacePrice.ToString(CultureInfo.InvariantCulture);
                    break;
                case "Total Quantity":
                    values[header] = TotalQuantity.ToString();
                    break;
                default:
                    values[header] = "";
                    break;
            }
        }
        
        // Return CSV row
        return string.Join(",", headers.Select(h => values.ContainsKey(h) ? values[h] : ""));
    }
}

class Program
{
    static void Main()
    {
        var inputPath = "../input.csv";
        var outputPath = "../cleaned_output.csv";

        Console.WriteLine("Reading CSV file...");
        var lines = File.ReadAllLines(inputPath);
        
        // Parse headers
        var headers = ParseCsvLine(lines[0]);
        Console.WriteLine($"Header has {headers.Length} columns");

        Console.WriteLine("Processing records...");
        var cleaned = new List<Card>();
        int errorCount = 0;
        int successCount = 0;
        
        for (int i = 1; i < lines.Length; i++)
        {
            try
            {
                string line = lines[i];
                var row = ParseCsvLine(line);
                
                if (row.Length != headers.Length)
                {
                    errorCount++;
                    if (errorCount <= 5) // Only show first few errors
                    {
                        Console.WriteLine($"Skipping row {i+1}: Column count mismatch (expected {headers.Length}, got {row.Length})");
                        Console.WriteLine($"Line: {line.Substring(0, Math.Min(50, line.Length))}...");
                    }
                    else if (errorCount == 6)
                    {
                        Console.WriteLine("Too many errors, suppressing further error messages...");
                    }
                    continue;
                }
                
                var card = new Card
                {
                    ProductName = GetValue(row, headers, "Product Name"),
                    TcgMarketPrice = ParseDouble(row, headers, "TCG Market Price"),
                    TcgLowPrice = ParseDouble(row, headers, "TCG Low Price"),
                    TcgLowWithShipping = ParseDouble(row, headers, "TCG Low Price With Shipping"),
                    TcgMarketplacePrice = ParseDouble(row, headers, "TCG Marketplace Price"),
                    TotalQuantity = ParseInt(row, headers, "Total Quantity")
                };
                
                if (card.TotalQuantity > 0)
                {
                    cleaned.Add(card);
                    successCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing row {i+1}: {ex.Message}");
            }
        }

        // Keep the original headers - don't add new ones
        var outputLines = new List<string> { string.Join(",", headers) };
        outputLines.AddRange(cleaned.Select(card => card.ToCsv(headers)));

        File.WriteAllLines(outputPath, outputLines);

        Console.WriteLine($"Processed {cleaned.Count} records successfully.");
        Console.WriteLine($"Skipped {errorCount} records due to parsing errors.");
        Console.WriteLine($"Output written to {outputPath}.");
        Console.WriteLine("TCG Marketplace Price has been replaced with Estimated Price where applicable.");
    }
    
    // Properly parse CSV line handling quoted fields with commas
    static string[] ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        StringBuilder field = new StringBuilder();
        bool inQuotes = false;
        
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            
            if (c == '"')
            {
                // Check if this is an escaped quote (double quote)
                if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    field.Append('"');
                    i++; // Skip the next quote
                }
                else
                {
                    // Toggle quote mode
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                // End of field
                result.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }
        
        // Add the last field
        result.Add(field.ToString());
        
        return result.ToArray();
    }
    
    static string GetValue(string[] row, string[] headers, string headerName)
    {
        int index = Array.IndexOf(headers, headerName);
        if (index < 0 || index >= row.Length)
            return "";
            
        return row[index];
    }
    
    static double ParseDouble(string[] row, string[] headers, string headerName)
    {
        string value = GetValue(row, headers, headerName);
        if (string.IsNullOrWhiteSpace(value))
            return 0.0;
            
        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            return result;
            
        return 0.0;
    }
    
    static int ParseInt(string[] row, string[] headers, string headerName)
    {
        string value = GetValue(row, headers, headerName);
        if (string.IsNullOrWhiteSpace(value))
            return 0;
            
        if (int.TryParse(value, out int result))
            return result;
            
        return 0;
    }
}
