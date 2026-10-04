using System.Text.RegularExpressions;
using AllamaShibliQuiz.Models.Blog;
using Markdig;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AllamaShibliQuiz.Services
{
    public interface IBlogService
    {
        IReadOnlyList<BlogPost> GetPublished();
        BlogPost? GetBySlug(string slug);
        BlogPost? GetBySlug(string slug, string lang);
        IReadOnlyList<BlogPost> GetByCategory(string categorySlug);
        IReadOnlyList<BlogPost> GetByTag(string tag);
        IReadOnlyList<BlogPost> GetRelated(BlogPost post, int count = 3);
        IReadOnlyList<BlogCategory> GetCategories();
        IReadOnlyList<string> GetAllTags();
    }

    /// <summary>
    /// Loads blog posts from Markdown files in Content/Blog once at startup and caches them in memory.
    /// English posts are named {slug}.md; translations are companion files {slug}.hi.md / {slug}.ur.md.
    /// Translations inherit category/tags/date/featuredImage from the English original when not specified.
    /// Registered as a singleton.
    /// </summary>
    public class BlogService : IBlogService
    {
        // All variants of all posts, across every language.
        private readonly List<BlogPost> _all;
        // English posts only (the canonical set used for listings), newest-first.
        private readonly List<BlogPost> _english;
        private readonly MarkdownPipeline _pipeline;

        private static readonly Regex FrontMatterRegex = new(
            @"^\s*---\s*\r?\n(?<yaml>.*?)\r?\n---\s*\r?\n(?<body>.*)$",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public BlogService(IWebHostEnvironment env, ILogger<BlogService> logger)
        {
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var contentDir = Path.Combine(env.ContentRootPath, "Content", "Blog");
            _all = LoadPosts(contentDir, deserializer, logger);
            LinkVariants(_all);

            _english = _all
                .Where(p => p.Language == BlogLanguages.Default)
                .OrderByDescending(p => p.Date)
                .ThenBy(p => p.Title)
                .ToList();
        }

        private List<BlogPost> LoadPosts(string contentDir, IDeserializer deserializer, ILogger logger)
        {
            var posts = new List<BlogPost>();
            if (!Directory.Exists(contentDir))
            {
                logger.LogWarning("Blog content directory not found: {Dir}", contentDir);
                return posts;
            }

            foreach (var file in Directory.GetFiles(contentDir, "*.md", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file).Equals("README.md", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                try
                {
                    var raw = File.ReadAllText(file);
                    var match = FrontMatterRegex.Match(raw);
                    if (!match.Success)
                    {
                        logger.LogWarning("Blog file has no front-matter, skipping: {File}", file);
                        continue;
                    }

                    var meta = deserializer.Deserialize<FrontMatter>(match.Groups["yaml"].Value)
                               ?? new FrontMatter();
                    var body = match.Groups["body"].Value;

                    // Derive language + base slug from the file name, e.g. "my-post.hi" → (hi, my-post).
                    var nameNoExt = Path.GetFileNameWithoutExtension(file); // strips ".md"
                    var (language, baseName) = SplitLanguage(nameNoExt);

                    // English may override its slug via front-matter; translations always use the
                    // file's base name so they reliably link to their English original.
                    var slug = language == BlogLanguages.Default && !string.IsNullOrWhiteSpace(meta.Slug)
                        ? meta.Slug.Trim()
                        : baseName;

                    var post = new BlogPost
                    {
                        Slug = slug,
                        Language = language,
                        Title = meta.Title?.Trim() ?? "Untitled",
                        Summary = meta.Summary?.Trim() ?? string.Empty,
                        Category = meta.Category?.Trim() ?? string.Empty,
                        Tags = meta.Tags ?? new List<string>(),
                        Date = meta.Date ?? File.GetLastWriteTime(file),
                        Author = string.IsNullOrWhiteSpace(meta.Author) ? "Team Brainiac Battle" : meta.Author.Trim(),
                        FeaturedImage = string.IsNullOrWhiteSpace(meta.FeaturedImage) ? null : meta.FeaturedImage.Trim(),
                        MetaTitle = meta.MetaTitle?.Trim(),
                        MetaDescription = meta.MetaDescription?.Trim(),
                        Published = meta.Published,
                        ContentHtml = Markdown.ToHtml(body, _pipeline),
                        WordCount = CountWords(body),
                    };
                    posts.Add(post);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to parse blog file: {File}", file);
                }
            }

            return posts;
        }

        /// <summary>Splits a file name like "my-post.hi" into ("hi", "my-post"); defaults to English.</summary>
        private static (string language, string baseName) SplitLanguage(string nameNoExt)
        {
            var dot = nameNoExt.LastIndexOf('.');
            if (dot > 0)
            {
                var suffix = nameNoExt[(dot + 1)..];
                if (BlogLanguages.IsSupported(suffix) &&
                    !suffix.Equals(BlogLanguages.Default, StringComparison.OrdinalIgnoreCase))
                {
                    return (suffix.ToLowerInvariant(), nameNoExt[..dot]);
                }
            }
            return (BlogLanguages.Default, nameNoExt);
        }

        /// <summary>
        /// Links translation variants to their English original: fills in inherited metadata,
        /// resolves each post's category, and records which languages each post is available in.
        /// </summary>
        private static void LinkVariants(List<BlogPost> all)
        {
            foreach (var group in all.GroupBy(p => p.Slug, StringComparer.OrdinalIgnoreCase))
            {
                var english = group.FirstOrDefault(p => p.Language == BlogLanguages.Default);
                var languages = group.Select(p => p.Language).Distinct().ToList();

                foreach (var post in group)
                {
                    post.AvailableLanguages = languages;

                    // Translations inherit language-neutral metadata from the English original.
                    if (post.IsTranslation && english != null)
                    {
                        if (string.IsNullOrEmpty(post.Category)) post.Category = english.Category;
                        if (post.Tags.Count == 0) post.Tags = english.Tags;
                        if (post.FeaturedImage == null) post.FeaturedImage = english.FeaturedImage;
                        if (post.Date == default) post.Date = english.Date;
                    }

                    post.CategoryInfo = BlogCategories.Find(post.Category);
                }
            }
        }

        private static int CountWords(string markdown) =>
            Regex.Matches(markdown, @"[\w'’]+").Count;

        public IReadOnlyList<BlogPost> GetPublished() =>
            _english.Where(p => p.Published).ToList();

        public BlogPost? GetBySlug(string slug) => GetBySlug(slug, BlogLanguages.Default);

        public BlogPost? GetBySlug(string slug, string lang) =>
            _all.FirstOrDefault(p => p.Published &&
                p.Language.Equals(lang, StringComparison.OrdinalIgnoreCase) &&
                p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

        public IReadOnlyList<BlogPost> GetByCategory(string categorySlug) =>
            _english.Where(p => p.Published &&
                p.Category.Equals(categorySlug, StringComparison.OrdinalIgnoreCase)).ToList();

        public IReadOnlyList<BlogPost> GetByTag(string tag) =>
            _english.Where(p => p.Published &&
                p.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase))).ToList();

        public IReadOnlyList<BlogPost> GetRelated(BlogPost post, int count = 3) =>
            _english.Where(p => p.Published && !p.Slug.Equals(post.Slug, StringComparison.OrdinalIgnoreCase))
                .Select(p => new
                {
                    Post = p,
                    Score = (p.Category.Equals(post.Category, StringComparison.OrdinalIgnoreCase) ? 2 : 0)
                            + p.Tags.Count(t => post.Tags.Contains(t, StringComparer.OrdinalIgnoreCase))
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Post.Date)
                .Take(count)
                .Select(x => x.Post)
                .ToList();

        public IReadOnlyList<BlogCategory> GetCategories() => BlogCategories.All;

        public IReadOnlyList<string> GetAllTags() =>
            _english.Where(p => p.Published)
                .SelectMany(p => p.Tags)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => t)
                .ToList();

        /// <summary>Raw front-matter shape deserialized from YAML (camelCase keys).</summary>
        private sealed class FrontMatter
        {
            public string? Title { get; set; }
            public string? Slug { get; set; }
            public string? Summary { get; set; }
            public string? Category { get; set; }
            public List<string>? Tags { get; set; }
            public DateTime? Date { get; set; }
            public string? Author { get; set; }
            public string? FeaturedImage { get; set; }
            public string? MetaTitle { get; set; }
            public string? MetaDescription { get; set; }
            public bool Published { get; set; } = true;
        }
    }
}
