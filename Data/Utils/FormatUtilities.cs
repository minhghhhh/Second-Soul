using System.Text.RegularExpressions;

namespace Data.Utils
{
    public class FormatUtilities
    {
        public static string TrimSpacesPreserveSingle(string input)
        {
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }
    }
}
