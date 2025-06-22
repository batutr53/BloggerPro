using System.Text.RegularExpressions;

namespace BloggerPro.Shared.Extensions
{
    public static class ContentHelper
    {
        public static string GenerateExcerpt(string content, int maxLength = 200)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var clean = Regex.Replace(content, "<.*?>", ""); // HTML tag temizleme
            return clean.Length > maxLength ? clean.Substring(0, maxLength) + "..." : clean;
        }
        public static string GenerateTitle(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "Untitled";

            var plainText = Regex.Replace(content, "<.*?>", string.Empty); // HTML etiketleri temizlenir
            return plainText.Split('.').FirstOrDefault()?.Trim() ?? "Untitled";
        }
    }

}
