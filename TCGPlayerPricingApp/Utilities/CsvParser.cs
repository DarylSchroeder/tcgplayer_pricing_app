using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TCGPlayerPricingApp.Utilities
{
    public class CsvParser
    {
        public static string[] ParseLine(string line)
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

        public static double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0.0;
                
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;
                
            return 0.0;
        }
        
        public static int ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;
                
            if (int.TryParse(value, out int result))
                return result;
                
            return 0;
        }
    }
}
