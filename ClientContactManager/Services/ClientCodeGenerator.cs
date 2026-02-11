using ClientContactManager.Data;
using Microsoft.EntityFrameworkCore;

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
        /// Generates a unique client code in format: AAA001
        /// First 3 characters are uppercase letters from client name (padded with 'A' if needed)
        /// Last 3 characters are sequential numbers (001-999)
        /// </summary>
        public async Task<string> GenerateClientCodeAsync(string clientName)
        {
            // Extract first 3 letters from name, convert to uppercase
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
            // Remove any non-letter characters and convert to uppercase
            string lettersOnly = new string(clientName.Where(char.IsLetter).ToArray()).ToUpper();

            if (lettersOnly.Length >= 3)
            {
                return lettersOnly.Substring(0, 3);
            }
            else if (lettersOnly.Length > 0)
            {
                // Pad with 'A' if less than 3 characters
                return lettersOnly.PadRight(3, 'A');
            }
            else
            {
                // If no letters at all, default to "AAA"
                return "AAA";
            }
        }

        private int FindNextAvailableNumber(List<string> existingCodes, string prefix)
        {
            if (!existingCodes.Any())
            {
                return 1;
            }

            // Extract numbers from existing codes
            var usedNumbers = existingCodes
                .Select(code => {
                    if (code.Length >= 6 && int.TryParse(code.Substring(3), out int num))
                    {
                        return num;
                    }
                    return 0;
                })
                .Where(num => num > 0)
                .OrderBy(num => num)
                .ToList();

            // Find first gap or next number
            int nextNumber = 1;
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
