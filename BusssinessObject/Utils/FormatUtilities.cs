using System.Text.RegularExpressions;

namespace BusssinessObject.Utils
{
    public class FormatUtilities
    {
        public static string TrimSpacesPreserveSingle(string input)
        {
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }
    }
}
