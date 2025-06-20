using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BloggerPro.Shared.Extensions;

public static class StringExtensions
{
    public static string GenerateSlug(this string phrase)
    {
        string str = phrase.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(str);
        str = Encoding.ASCII.GetString(bytes);

        str = Regex.Replace(str, @"\s", "-", RegexOptions.Compiled);
        str = Regex.Replace(str, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);
        str = Regex.Replace(str, @"([-]){2,}", "$1", RegexOptions.Compiled).Trim('-');

        return str;
    }
}
