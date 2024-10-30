using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.RegularExpressions;

namespace Data.Utils
{
    public class FormatUtilities
    {
        public static string TrimSpacesPreserveSingle(string input)
        {
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }

		public static string GenerateRandomCodeWithExpiration(int minutesToExpire)
		{
			var tokenId = Guid.NewGuid().ToString();
			var expirationDate = DateTime.UtcNow.AddMinutes(minutesToExpire);

			var token = $"{tokenId}|{expirationDate:o}";

			return token;
		}

		public static bool ValidateToken(string token)
		{

			var parts = token.Split('|');
			if (parts.Length != 2)
				return false;

			var tokenId = parts[0];
			if (!DateTime.TryParse(parts[1], out var expirationDate))
				return false;

			if (DateTime.UtcNow > expirationDate)
				return false;

			return true;
		}

	}
}
