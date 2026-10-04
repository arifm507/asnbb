using AllamaShibliQuiz.Models.Blog;

namespace AllamaShibliQuiz.Models.ViewModels
{
    /// <summary>Backing model for the blog listing, category, and tag pages.</summary>
    public class BlogListViewModel
    {
        public IReadOnlyList<BlogPost> Posts { get; set; } = new List<BlogPost>();
        public IReadOnlyList<BlogCategory> Categories { get; set; } = new List<BlogCategory>();

        public int Page { get; set; } = 1;
        public int TotalPages { get; set; } = 1;

        /// <summary>Heading shown on the page (blog title, category name, or "Tag: …").</summary>
        public string Heading { get; set; } = "Blog";
        public string Subtitle { get; set; } = string.Empty;

        /// <summary>Active category slug when viewing a category page, otherwise null.</summary>
        public string? ActiveCategory { get; set; }

        /// <summary>Active tag when viewing a tag page, otherwise null.</summary>
        public string? ActiveTag { get; set; }

        // SEO
        public string MetaTitle { get; set; } = string.Empty;
        public string MetaDescription { get; set; } = string.Empty;

        /// <summary>Route values ("category"/"tag" slug) used to build pagination links.</summary>
        public string PageKind { get; set; } = "index"; // index | category | tag
        public string? PageRouteValue { get; set; }
    }
}
