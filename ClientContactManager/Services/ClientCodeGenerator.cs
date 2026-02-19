using ClientContactManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ClientContactManager.Services
{
    public class ClientCodeGenerator
    {
        private readonly ApplicationDbContext _context;

        public ClientCodeGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generates a unique client code in format: AAA100
        /// First 3 characters are uppercase initials/letters from client name (padded with 'A' if needed)
        /// Last 3 characters are sequential numbers (100-999)
        /// </summary>
        public async Task<string> GenerateClientCodeAsync(string clientName)
        {
            // Extract prefix from name
            string prefix = ExtractPrefix(clientName);

            // Find existing codes with this prefix
            var existingCodes = await _context.Clients
                .Where(c => c.ClientCode.StartsWith(prefix))
                .Select(c => c.ClientCode)
                .ToListAsync();

            // Find next available number
            int nextNumber = FindNextAvailableNumber(existingCodes, prefix);

            // Format: ABC123
            return $"{prefix}{nextNumber:D3}";
        }

        private string ExtractPrefix(string clientName)
        {
            var words = Regex.Matches(clientName.ToUpperInvariant(), "[A-Z]+")
                .Select(m => m.Value)
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .ToList();

            if (words.Count >= 2)
            {
                // Multi-word names: initials from first 3 words (e.g., First National Bank -> FNB)
                return string.Concat(words.Take(3).Select(w => w[0])).PadRight(3, 'A');
            }

            if (words.Count == 1)
            {
                // Single-word names: first 3 letters, padded if needed
                return words[0].PadRight(3, 'A').Substring(0, 3);
            }

            // If no letters at all, default to "AAA"
            return "AAA";
        }

        private int FindNextAvailableNumber(List<string> existingCodes, string prefix)
        {
            const int start = 100;

            var usedNumbers = existingCodes
                .Where(code => code.Length == 6 && code.StartsWith(prefix))
                .Select(code => int.TryParse(code.Substring(3, 3), out int num) ? num : 0)
                .Where(num => num >= start && num <= 999)
                .OrderBy(num => num)
                .ToList();

            // Find first gap or next number
            int nextNumber = start;
            foreach (var num in usedNumbers)
            {
                if (num == nextNumber)
                {
                    nextNumber++;
                }
                else if (num > nextNumber)
                {
                    break;
                }
            }

            // Maximum number is 999
            if (nextNumber > 999)
            {
                throw new InvalidOperationException($"Maximum client codes reached for prefix {prefix}");
            }

            return nextNumber;
        }
    }
}
