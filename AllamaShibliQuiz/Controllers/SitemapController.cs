using System.Xml;
using AllamaShibliQuiz.Services;
using Microsoft.AspNetCore.Mvc;

namespace AllamaShibliQuiz.Controllers
{
    /// <summary>Serves a dynamic XML sitemap at /sitemap.xml covering public pages and all blog content.</summary>
    public class SitemapController : Controller
    {
        private readonly IBlogService _blog;

        public SitemapController(IBlogService blog)
        {
            _blog = blog;
        }

        [HttpGet("/sitemap.xml")]
        public IActionResult Index()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            using var stream = new MemoryStream();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
            };
            using (var writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                writer.WriteAttributeString("xmlns", "xhtml", null, "http://www.w3.org/1999/xhtml");

                // Static public pages
                WriteUrl(writer, $"{baseUrl}/", "1.0", "weekly");
                WriteUrl(writer, $"{baseUrl}/Blog", "0.9", "daily");
                WriteUrl(writer, $"{baseUrl}/Gallery", "0.6", "monthly");

                // Blog categories
                foreach (var c in _blog.GetCategories())
                {
                    WriteUrl(writer, $"{baseUrl}/blog/category/{c.Slug}", "0.7", "weekly");
                }

                // Blog posts (one entry per available language, cross-linked via hreflang)
                foreach (var post in _blog.GetPublished())
                {
                    var lastMod = post.Date.ToString("yyyy-MM-dd");
                    foreach (var code in post.AvailableLanguages)
                    {
                        writer.WriteStartElement("url");
                        writer.WriteElementString("loc", PostUrl(baseUrl, code, post.Slug));
                        writer.WriteElementString("lastmod", lastMod);
                        writer.WriteElementString("changefreq", "monthly");
                        writer.WriteElementString("priority", "0.8");
                        foreach (var alt in post.AvailableLanguages)
                        {
                            WriteAlternate(writer, alt, PostUrl(baseUrl, alt, post.Slug));
                        }
                        WriteAlternate(writer, "x-default", PostUrl(baseUrl, "en", post.Slug));
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return File(stream.ToArray(), "application/xml");
        }

        [HttpGet("/robots.txt")]
        public IActionResult Robots()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var body = $"User-agent: *\nAllow: /\nDisallow: /Admin\n\nSitemap: {baseUrl}/sitemap.xml\n";
            return Content(body, "text/plain", System.Text.Encoding.UTF8);
        }

        private static string PostUrl(string baseUrl, string lang, string slug) =>
            lang == "en" ? $"{baseUrl}/blog/{slug}" : $"{baseUrl}/blog/{lang}/{slug}";

        private static void WriteAlternate(XmlWriter writer, string hreflang, string href)
        {
            writer.WriteStartElement("xhtml", "link", "http://www.w3.org/1999/xhtml");
            writer.WriteAttributeString("rel", "alternate");
            writer.WriteAttributeString("hreflang", hreflang);
            writer.WriteAttributeString("href", href);
            writer.WriteEndElement();
        }

        private static void WriteUrl(XmlWriter writer, string loc, string priority,
            string changeFreq, string? lastMod = null)
        {
            writer.WriteStartElement("url");
            writer.WriteElementString("loc", loc);
            if (lastMod != null)
            {
                writer.WriteElementString("lastmod", lastMod);
            }
            writer.WriteElementString("changefreq", changeFreq);
            writer.WriteElementString("priority", priority);
            writer.WriteEndElement();
        }
    }
}
