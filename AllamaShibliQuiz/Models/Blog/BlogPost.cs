namespace AllamaShibliQuiz.Models.Blog
{
    /// <summary>
    /// A single blog post parsed from a Markdown file in Content/Blog.
    /// Metadata comes from the YAML front-matter; <see cref="ContentHtml"/> is the rendered body.
    /// </summary>
    public class BlogPost
    {
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;

        /// <summary>Language code of this variant: en (default), hi, or ur.</summary>
        public string Language { get; set; } = BlogLanguages.Default;

        /// <summary>Language codes this post is available in (always includes en). Set by the service.</summary>
        public List<string> AvailableLanguages { get; set; } = new() { BlogLanguages.Default };

        /// <summary>True when this variant is a translation of the English original.</summary>
        public bool IsTranslation => Language != BlogLanguages.Default;

        /// <summary>Category slug (matches a <see cref="BlogCategory.Slug"/>).</summary>
        public string Category { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new();
        public DateTime Date { get; set; }
        public string Author { get; set; } = "Team Brainiac Battle";

        /// <summary>Public URL of the featured image, e.g. /uploads/blog/shibli.jpg.</summary>
        public string? FeaturedImage { get; set; }

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public bool Published { get; set; } = true;

        /// <summary>Rendered HTML of the Markdown body.</summary>
        public string ContentHtml { get; set; } = string.Empty;

        /// <summary>Plain-text word count of the body, used for reading-time estimates.</summary>
        public int WordCount { get; set; }

        /// <summary>Estimated reading time in minutes (≈200 words/minute, minimum 1).</summary>
        public int ReadingTimeMinutes => Math.Max(1, (int)Math.Ceiling(WordCount / 200.0));

        /// <summary>Resolved category details, set by the service after parsing.</summary>
        public BlogCategory? CategoryInfo { get; set; }
    }
}
