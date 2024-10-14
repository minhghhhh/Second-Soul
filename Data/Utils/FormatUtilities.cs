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

		public static string RemoveLastParagraph(string input)
		{
			int lastIndex = input.LastIndexOf("\n\n");

			if (lastIndex == -1)
			{
				return input;
			}

			return input[..lastIndex].TrimEnd();
		}

		public static string ExtractLastParagraph(string input)
		{
			int lastIndex = input.LastIndexOf("\n\n");

			if (lastIndex == -1)
			{
				return string.Empty;
			}

			return input[lastIndex..];
		}
	}
}
