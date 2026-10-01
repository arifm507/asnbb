# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **AllamaShibliQuiz** — an ASP.NET Core 10 MVC web application for managing student registrations for a competitive quiz exam (ASNBB). It handles public student registration, admin approval workflows, roll number generation, and serves static content (syllabi, answer keys, results).

## Common Commands

```bash
# Run locally
dotnet run

# Build
dotnet build

# Apply EF Core migrations manually (also runs automatically on startup)
dotnet ef database update

# Add a migration
dotnet ef migrations add <MigrationName>
```

Default local URLs: `https://localhost:7177` / `http://localhost:5177`

## Configuration

The connection string is set via environment variable `ASNBB_CONNECTION_STRING` (falls back to `appsettings.Development.json`):

```
Host=localhost;Port=5432;Database=asnbb;Username=postgres;Password=postgres;
```

Migrations run automatically at startup via `db.Database.MigrateAsync()` in `Program.cs`.

## Architecture

**Stack:** ASP.NET Core 10 MVC · PostgreSQL · Entity Framework Core · AutoMapper · Cookie authentication

**Data layer:** Single `AsnbbDBContext` (`Data/AsnbbDBContext.cs`) with 4 entities:
- `Student` — registrant data including status (`Pending`/`Approved`/`Rejected`), exam centre, and roll number
- `School` — schools and exam centres; `IsExamCentre`/`IsExternalExamCentre` flags determine exam centre eligibility
- `Team` — team member profiles with image storage
- `AdminUser` — admin credentials (AES-encrypted password via `Helpers/StringExtension.cs`)

**Controllers:**
- `HomeController` — public pages
- `RegisterController` — student registration form, duplicate detection, and registration history lookup
- `AdminController` — [Authorize] protected; student/team CRUD, dashboard stats, bulk approval, roll number generation
- `AnswerKeyController`, `ResultController`, `SyllabusController` — serve files from `wwwroot/` subdirectories
- `AdmitCardController` — disabled with a status message until a configured date
- `GalleryController` — gallery view

**Roll number format:** `26{CentreCode}{Class}{SequenceNumber}` (e.g., `260102001`). Generated in `AdminController` on approval.

**ViewModels** map to/from entities via AutoMapper (`Profiles/UserProfile.cs`). All views use strongly-typed view models; request payloads use separate request models in `Models/RequestModels/`.

## Static Content

Answer keys, results, and syllabi are served directly from `wwwroot/`:
- `wwwroot/answer_key/` — PDFs
- `wwwroot/results/2024/` — result PDFs by class
- `wwwroot/syllabus/` — JPEG images by class

Team member images are stored in `wwwroot/uploads/` (max 200 KB, checked in controller).

## Deployment

A `Dockerfile` is present for containerized deployment. The app is hosted on Render (see commit history re: inotify polling fix for file watcher). Production uses the `ASNBB_CONNECTION_STRING` environment variable.
