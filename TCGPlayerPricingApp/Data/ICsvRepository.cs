using System.Collections.Generic;
using TCGPlayerPricingApp.Models;

namespace TCGPlayerPricingApp.Data
{
    public interface ICsvRepository
    {
        (string[] Headers, List<Card> Cards) ReadCards(string filePath);
        void WriteCards(string filePath, string[] headers, IEnumerable<Card> cards);
    }
}
