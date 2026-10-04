namespace AllamaShibliQuiz.Models.Blog
{
    /// <summary>
    /// A blog category. The fixed set is defined in <see cref="BlogCategories.All"/>.
    /// </summary>
    public class BlogCategory
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        /// <summary>Short description shown on the category landing page and used as its meta description.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Bootstrap Icons class for the category chip, e.g. "bi-person-badge".</summary>
        public string Icon { get; set; } = "bi-journal-text";
    }

    /// <summary>
    /// The fixed, curated set of blog categories for the site.
    /// A post's <c>category</c> front-matter value must match one of these slugs.
    /// </summary>
    public static class BlogCategories
    {
        public static readonly IReadOnlyList<BlogCategory> All = new List<BlogCategory>
        {
            new() {
                Slug = "allama-shibli-nomani",
                Name = "Allama Shibli Nomani",
                Icon = "bi-person-vcard",
                Description = "The life, works, and enduring legacy of Allama Shibli Nomani — the celebrated scholar, poet, and historian of Azamgarh."
            },
            new() {
                Slug = "personalities-of-azamgarh",
                Name = "Personalities of Azamgarh",
                Icon = "bi-people",
                Description = "Stories of the poets, freedom fighters, scholars, and leaders who shaped Azamgarh and made it a cradle of learning."
            },
            new() {
                Slug = "uttar-pradesh-history",
                Name = "Uttar Pradesh History",
                Icon = "bi-bank",
                Description = "Explore the rich history, heritage, and culture of Uttar Pradesh — from ancient kingdoms to the modern day."
            },
            new() {
                Slug = "exam-preparation",
                Name = "Exam Preparation",
                Icon = "bi-mortarboard",
                Description = "Practical tips, strategies, and study plans to help students crack competitive exams with confidence."
            }
        };

        public static BlogCategory? Find(string? slug) =>
            string.IsNullOrWhiteSpace(slug)
                ? null
                : All.FirstOrDefault(c => c.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    }
}
