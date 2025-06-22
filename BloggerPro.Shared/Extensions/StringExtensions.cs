using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BloggerPro.Shared.Extensions;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string GenerateSlug(this string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
            return string.Empty;

        string str = phrase.ToLowerInvariant().Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder();
        foreach (var c in str)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        str = sb.ToString().Normalize(NormalizationForm.FormC);

        // Boşlukları tireye çevir
        str = Regex.Replace(str, @"\s+", "-", RegexOptions.Compiled);

        // Geçersiz karakterleri sil
        str = Regex.Replace(str, @"[^a-z0-9\-]", "", RegexOptions.Compiled);

        // Art arda gelen tireleri tek tireye indir
        str = Regex.Replace(str, @"-+", "-", RegexOptions.Compiled).Trim('-');

        return str;
    }
}

