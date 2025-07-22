# TCG Player Pricing App

A C# application for processing TCG Player pricing data and calculating estimated prices based on specific pricing rules.

## Overview

This application reads a CSV file exported from TCG Player containing card pricing data, processes each card according to defined pricing rules, and outputs a new CSV file with the TCG Marketplace Price column replaced with calculated Estimated Prices.

## Features

- Processes TCG Player CSV exports
- Applies custom pricing rules based on card value
- Handles quoted CSV fields with commas
- Filters out cards with zero quantity
- Maintains the original CSV structure

## Pricing Rules

The application applies the following pricing rules:

1. **Expensive Cards** (TCG Market Price or TCG Marketplace Price > $30):
   - Keep the original TCG Marketplace Price

2. **Very Cheap Cards** (TCG Market Price ≤ $0.30):
   - Price = max($0.50, TCG Low Price)

3. **Standard Cards** (TCG Market Price between $0.30 and $30):
   - Calculate average of (TCG Low Price With Shipping + TCG Market Price) / 2
   - Price = max($0.50, max(TCG Low Price, calculated average))

4. **General Rules**:
   - All prices are rounded to 2 decimal places
   - Minimum price for any card is $0.50
   - Cards with zero quantity are excluded from output

## Requirements

- .NET SDK 8.0 or later
- Input CSV file with the following columns:
  - Product Name
  - TCG Market Price
  - TCG Low Price
  - TCG Low Price With Shipping
  - TCG Marketplace Price
  - Total Quantity

## Usage

1. Place your TCG Player export CSV file in the application directory as `input.csv`
2. Run the application:
   ```
   cd TCGPlayerPricingApp
   dotnet run
   ```
3. The processed data will be saved as `cleaned_output.csv` in the application directory

## CSV Format

The application expects a CSV file with headers matching those from TCG Player exports. The key columns used for pricing calculations are:

- TCG Market Price
- TCG Low Price
- TCG Low Price With Shipping
- TCG Marketplace Price
- Total Quantity

## Development

### Project Structure

- `Program.cs`: Contains the main application logic and CSV parsing
- `Card.cs`: Defines the Card class with pricing logic (included in Program.cs)

### Building from Source

```bash
# Clone the repository
git clone <repository-url>

# Navigate to the project directory
cd tcgplayer_pricing_app/TCGPlayerPricingApp

# Build the project
dotnet build

# Run the application
dotnet run
```

## Troubleshooting

- **CSV Parsing Errors**: The application includes robust CSV parsing to handle quoted fields with commas. If you encounter parsing issues, check that your CSV file uses standard formatting.
- **Missing Columns**: Ensure your input CSV contains all required columns for price calculations.
