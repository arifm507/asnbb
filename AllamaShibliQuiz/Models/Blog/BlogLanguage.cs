namespace AllamaShibliQuiz.Models.Blog
{
    /// <summary>A language the blog can be served in.</summary>
    public class BlogLanguage
    {
        /// <summary>Two-letter code: en, hi, ur.</summary>
        public string Code { get; set; } = "en";

        /// <summary>Native display name shown in the language toggle (e.g. हिंदी).</summary>
        public string NativeName { get; set; } = string.Empty;

        /// <summary>English name, used for hreflang labels / accessibility.</summary>
        public string EnglishName { get; set; } = string.Empty;

        /// <summary>True for right-to-left scripts (Urdu).</summary>
        public bool IsRtl { get; set; }
    }

    /// <summary>The fixed set of languages the blog supports.</summary>
    public static class BlogLanguages
    {
        public const string Default = "en";

        public static readonly IReadOnlyList<BlogLanguage> All = new List<BlogLanguage>
        {
            new() { Code = "en", NativeName = "English", EnglishName = "English", IsRtl = false },
            new() { Code = "hi", NativeName = "हिंदी",   EnglishName = "Hindi",   IsRtl = false },
            new() { Code = "ur", NativeName = "اردو",    EnglishName = "Urdu",    IsRtl = true  },
        };

        public static bool IsSupported(string? code) =>
            !string.IsNullOrWhiteSpace(code) &&
            All.Any(l => l.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        public static BlogLanguage Get(string code) =>
            All.FirstOrDefault(l => l.Code.Equals(code, StringComparison.OrdinalIgnoreCase)) ?? All[0];
    }
}
