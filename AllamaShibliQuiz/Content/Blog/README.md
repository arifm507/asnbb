# Blog content

Every file in this folder (and subfolders) ending in `.md` becomes a blog post.
Posts are loaded into memory once when the app starts, so **adding a new post means
committing a `.md` file and redeploying**.

## Front-matter

Each file must begin with a YAML front-matter block delimited by `---`:

```markdown
---
title: Who Was Allama Shibli Nomani?
slug: who-was-allama-shibli-nomani
category: allama-shibli-nomani
tags: [history, scholar, azamgarh]
date: 2026-10-02
author: Team Brainiac Battle
summary: A short one-paragraph excerpt shown on cards and used as the fallback meta description.
metaTitle: Allama Shibli Nomani — Life, Works & Legacy   # optional; overrides the browser <title>
metaDescription: The life and legacy of Allama Shibli Nomani.  # optional; overrides summary for SEO
featuredImage: /uploads/blog/shibli.jpg                  # optional; public URL under wwwroot
published: true                                          # set false to hide a draft
---

## Your content starts here
Write the body in **Markdown**.
```

### Fields

| Field            | Required | Notes                                                                 |
|------------------|----------|-----------------------------------------------------------------------|
| `title`          | yes      | Post headline.                                                        |
| `slug`           | no       | URL segment (`/blog/{slug}`). Defaults to the file name.             |
| `category`       | yes      | Must match a category slug (see below).                               |
| `tags`           | no       | List of freeform tags; each gets a `/blog/tag/{tag}` page.            |
| `date`           | no       | Publish date. Defaults to the file's last-modified time.             |
| `author`         | no       | Defaults to "Team Brainiac Battle".                                   |
| `summary`        | yes      | Card excerpt + fallback meta description.                             |
| `metaTitle`      | no       | SEO `<title>`; falls back to `title`.                                 |
| `metaDescription`| no       | SEO meta description; falls back to `summary`.                        |
| `featuredImage`  | no       | Image URL. Put files in `wwwroot/uploads/blog/`.                      |
| `published`      | no       | `true` by default; `false` keeps the post hidden.                    |

## Category slugs

- `allama-shibli-nomani`
- `personalities-of-azamgarh`
- `uttar-pradesh-history`
- `exam-preparation`

(Defined in `Models/Blog/BlogCategory.cs`.)

## Translations (Hindi / Urdu)

A post can be offered in Hindi and Urdu by adding **companion files** next to the English one,
named with a language suffix:

```
how-to-crack-competitive-exams.md       <- English (original)
how-to-crack-competitive-exams.hi.md    <- Hindi translation
how-to-crack-competitive-exams.ur.md    <- Urdu translation
```

- The file name (minus the `.hi` / `.ur` suffix) **must match the English post's slug** — that is
  how a translation links to its original.
- A translation only needs `title`, `summary`, and the translated body in its front-matter.
  `category`, `tags`, `date`, and `featuredImage` are **inherited from the English post** if omitted.
- Supported languages: `en` (default), `hi`, `ur`. Urdu is rendered right-to-left automatically.
- A language toggle (English · हिंदी · اردو) appears on the article automatically for whichever
  translations exist. Missing languages are simply not shown.
- URLs: English `/blog/{slug}`, Hindi `/blog/hi/{slug}`, Urdu `/blog/ur/{slug}`. Each is a real,
  separately-indexable page and the three are cross-linked with `hreflang` tags for SEO.

Translations are written/pasted by hand for now (so you can review quality before publishing).
A machine-translation step can be added later to pre-generate these drafts.

### Minimal translation file example

```markdown
---
title: प्रतियोगी परीक्षाओं को कैसे पास करें
summary: स्मार्ट तैयारी कठिन तैयारी से बेहतर है। यहाँ कुछ व्यावहारिक रणनीतियाँ दी गई हैं।
---

## परीक्षा को समझें
कोई भी किताब खोलने से पहले, **पाठ्यक्रम और पिछले प्रश्नपत्रों** का अध्ययन करें...
```

## Images

Store images in `wwwroot/uploads/blog/` and reference them by public URL, e.g.
`/uploads/blog/my-image.jpg`. Always add descriptive `alt` text in Markdown:
`![Allama Shibli Nomani portrait](/uploads/blog/shibli.jpg)`.
