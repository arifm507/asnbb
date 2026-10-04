using AllamaShibliQuiz.Models.Blog;
using AllamaShibliQuiz.Models.ViewModels;
using AllamaShibliQuiz.Services;
using Microsoft.AspNetCore.Mvc;

namespace AllamaShibliQuiz.Controllers
{
    [Route("blog")]
    public class BlogController : Controller
    {
        private const int PageSize = 9;
        private readonly IBlogService _blog;

        public BlogController(IBlogService blog)
        {
            _blog = blog;
        }

        // GET /blog
        [HttpGet("")]
        public IActionResult Index(int page = 1)
        {
            var all = _blog.GetPublished();
            var model = BuildList(all, page, kind: "index", routeValue: null,
                heading: "Our Blog",
                subtitle: "Stories, history, and study tips from the Brainiac Battle team.");
            model.MetaTitle = "Blog — Allama Shibli Nomani Brainiac Battle";
            model.MetaDescription = "Read about Allama Shibli Nomani, the personalities of Azamgarh, the history of Uttar Pradesh, and tips to crack competitive exams.";
            return View(model);
        }

        // GET /blog/category/{slug}
        [HttpGet("category/{slug}")]
        public IActionResult Category(string slug, int page = 1)
        {
            var category = BlogCategories.Find(slug);
            if (category == null)
            {
                return NotFound();
            }
            var posts = _blog.GetByCategory(slug);
            var model = BuildList(posts, page, kind: "category", routeValue: slug,
                heading: category.Name,
                subtitle: category.Description);
            model.ActiveCategory = slug;
            model.MetaTitle = $"{category.Name} — ASNBB Blog";
            model.MetaDescription = category.Description;
            return View("Index", model);
        }

        // GET /blog/tag/{tag}
        [HttpGet("tag/{tag}")]
        public IActionResult Tag(string tag, int page = 1)
        {
            var posts = _blog.GetByTag(tag);
            if (posts.Count == 0)
            {
                return NotFound();
            }
            var model = BuildList(posts, page, kind: "tag", routeValue: tag,
                heading: $"#{tag}",
                subtitle: $"Posts tagged \"{tag}\".");
            model.ActiveTag = tag;
            model.MetaTitle = $"Posts tagged {tag} — ASNBB Blog";
            model.MetaDescription = $"Browse blog posts tagged {tag} on the Allama Shibli Nomani Brainiac Battle website.";
            return View("Index", model);
        }

        // GET /blog/{slug}  (English)
        [HttpGet("{slug}")]
        public IActionResult Details(string slug) => RenderPost(slug, BlogLanguages.Default);

        // GET /blog/hi/{slug} or /blog/ur/{slug}  (translations)
        [HttpGet("{lang:regex(^(hi|ur)$)}/{slug}")]
        public IActionResult DetailsLocalized(string lang, string slug) => RenderPost(slug, lang);

        private IActionResult RenderPost(string slug, string lang)
        {
            var post = _blog.GetBySlug(slug, lang);
            if (post == null)
            {
                return NotFound();
            }
            // Related reads are shown from the English set (links resolve to each post's default language).
            ViewBag.Related = _blog.GetRelated(post);
            return View("Details", post);
        }

        private BlogListViewModel BuildList(IReadOnlyList<BlogPost> source, int page,
            string kind, string? routeValue, string heading, string subtitle)
        {
            if (page < 1) page = 1;
            var totalPages = Math.Max(1, (int)Math.Ceiling(source.Count / (double)PageSize));
            if (page > totalPages) page = totalPages;

            var paged = source
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new BlogListViewModel
            {
                Posts = paged,
                Categories = _blog.GetCategories(),
                Page = page,
                TotalPages = totalPages,
                Heading = heading,
                Subtitle = subtitle,
                PageKind = kind,
                PageRouteValue = routeValue,
            };
        }
    }
}
