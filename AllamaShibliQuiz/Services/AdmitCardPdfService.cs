using System.IO.Compression;
using System.Text.RegularExpressions;
using AllamaShibliQuiz.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AllamaShibliQuiz.Services
{
    public interface IAdmitCardPdfService
    {
        byte[] CreateZip(IReadOnlyList<StudentViewModel> students);
    }

    /// <summary>
    /// Builds printable admit-card PDFs that follow Views/AdmitCard/ViewCard.cshtml.
    /// </summary>
    public class AdmitCardPdfService : IAdmitCardPdfService
    {
        private static readonly string[] Instructions =
        {
            "Candidate should report 30 minutes before examination time.",
            "Candidate should bring their own blue/black ball pen for filling OMR sheets.",
            "There will be negative mark of .25 on every wrong answer.",
            "Candidate without a proper admit card and school ID card shall not be allowed in the examination.",
            "Candidate shall not be allowed to leave the examination hall before the conclusion of examination.",
            "Use of WHITE FLUID & OVER WRITING/CUTTING on TEST BOOKLET and OMR SHEET is strictly prohibited.",
            "Candidate must carry — i) ADMIT CARD   ii) SCHOOL ID CARD   iii) BALL POINT PEN (BLUE/BLACK)"
        };

        private readonly byte[]? _logo;
        private readonly byte[]? _presidentSign;
        private readonly byte[]? _controllerSign;

        static AdmitCardPdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.ThrowOnMissingTextGlyphs = false;
        }

        public AdmitCardPdfService(IWebHostEnvironment environment)
        {
            var imageRoot = Path.Combine(environment.WebRootPath, "assets", "img");
            _logo = TryRead(Path.Combine(imageRoot, "logo.png"));
            _presidentSign = TryRead(Path.Combine(imageRoot, "asad_sign.jpeg"));
            _controllerSign = TryRead(Path.Combine(imageRoot, "ahmar_sign.jpeg"));
        }

        public byte[] CreateZip(IReadOnlyList<StudentViewModel> students)
        {
            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var student in students)
                {
                    var entry = archive.CreateEntry(PdfFileName(student, usedNames), CompressionLevel.Fastest);
                    using var entryStream = entry.Open();
                    using var pdfStream = new MemoryStream();
                    Generate(student, pdfStream);
                    pdfStream.Position = 0;
                    pdfStream.CopyTo(entryStream);
                }
            }

            return zipStream.ToArray();
        }

        private void Generate(StudentViewModel student, Stream output)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(28);
                    page.MarginVertical(24);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(style => style.FontSize(10).FontColor(Colors.Black));
                    page.Content().Element(content => ComposeCard(content, student));
                });
            }).GeneratePdf(output);
        }

        private void ComposeCard(IContainer container, StudentViewModel student)
        {
            container.Border(1).BorderColor(Colors.Grey.Darken2).Padding(14).Column(column =>
            {
                column.Spacing(1);
                column.Item().AlignCenter().Row(row =>
                {
                    var logo = _logo;
                    if (logo != null)
                    {
                        row.ConstantItem(46).Height(46).AlignMiddle().Image(logo).FitArea();
                        row.ConstantItem(8);
                    }

                    row.RelativeItem().AlignMiddle().AlignCenter()
                        .Text("Allama Shibli Nomani Brainiac Battle")
                        .Bold().FontSize(15);
                });

                column.Item().AlignCenter().Text("Season - 3").Bold().FontSize(13);
                column.Item().AlignCenter().Text("Organized by - Team Brainiac Battle");
                column.Item().AlignCenter().Text("Sponsored by - Moral Public School, Chittepur, Saraimeer");
                column.Item().AlignCenter().Text("(Admit Card - 2026)");

                column.Item().PaddingTop(8).Element(content => ComposeStudentTable(content, student));
                column.Item().Element(content => ComposeExamTable(content, student));
                column.Item().Element(ComposeSignatures);

                column.Item().PaddingTop(8).Text("Important Instructions for candidates:-")
                    .Bold().FontSize(11).FontColor(Colors.Red.Darken2);

                for (var i = 0; i < Instructions.Length; i++)
                {
                    var line = $"{i + 1}. {Instructions[i]}";
                    column.Item().PaddingTop(1).Text(line).FontSize(9);
                }
            });
        }

        private static void ComposeStudentTable(IContainer container, StudentViewModel student)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(145);
                    columns.RelativeColumn();
                    columns.ConstantColumn(125);
                });

                table.Cell().Element(LabelCell).Text("Name");
                table.Cell().ColumnSpan(2).Element(ValueCell).Text(Display(student.Name));

                table.Cell().Element(LabelCell).Text("Roll Number");
                table.Cell().Element(ValueCell).Text(Display(student.RollNumber));
                table.Cell().RowSpan(4).Element(ValueCell).MinHeight(96)
                    .AlignCenter().AlignMiddle()
                    .Text("Please affix your passport size photograph here.")
                    .FontSize(8).FontColor(Colors.Grey.Darken1).AlignCenter();

                table.Cell().Element(LabelCell).Text("Father's Name");
                table.Cell().Element(ValueCell).Text(Display(student.FatherName));

                table.Cell().Element(LabelCell).Text("Class");
                table.Cell().Element(ValueCell).Text(student.Class.ToString());

                table.Cell().Element(LabelCell).Text("School Name");
                table.Cell().Element(ValueCell).Text(Display(student.SchoolName));
            });
        }

        private static void ComposeExamTable(IContainer container, StudentViewModel student)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(145);
                    columns.RelativeColumn();
                    columns.ConstantColumn(125);
                });

                table.Cell().ColumnSpan(3).Element(LabelCell).AlignCenter()
                    .Text("EXAM DAY DETAILS").Bold();

                table.Cell().Element(LabelCell).Text("Date and Day of Exam");
                table.Cell().Element(ValueCell).Text(Display(student.ExamDate));
                table.Cell().RowSpan(3).Element(ValueCell);

                table.Cell().Element(LabelCell).Text("Exam Timing");
                table.Cell().Element(ValueCell).Text(ExamTiming(student.ExamCentreId));

                table.Cell().Element(LabelCell).Text("Center Code");
                table.Cell().Element(ValueCell).Text(Display(student.ExamCentreCode));

                table.Cell().Element(LabelCell).Text("Exam Center");
                table.Cell().Element(ValueCell).Text(Display(student.ExamCentreName));
                table.Cell().Element(ValueCell).MinHeight(42).AlignCenter().AlignMiddle()
                    .Text("Student Sign").FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        }

        private void ComposeSignatures(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Element(ValueCell).Height(58).AlignCenter().AlignMiddle()
                    .Element(cell => DrawSignature(cell, _presidentSign));
                table.Cell().Element(ValueCell).Height(58).AlignCenter().AlignMiddle()
                    .Element(cell => DrawSignature(cell, _controllerSign));

                table.Cell().Element(ValueCell).AlignCenter()
                    .Text("President Signature").FontSize(9).FontColor(Colors.Grey.Darken1);
                table.Cell().Element(ValueCell).AlignCenter()
                    .Text("Examination Controller Signature").FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        }

        private static void DrawSignature(IContainer container, byte[]? image)
        {
            if (image == null)
            {
                return;
            }

            container.Height(46).AlignCenter().Image(image).FitHeight();
        }

        private static string PdfFileName(StudentViewModel student, HashSet<string> usedNames)
        {
            var roll = string.IsNullOrWhiteSpace(student.RollNumber) ? "NoRoll" : student.RollNumber.Trim();
            var name = string.IsNullOrWhiteSpace(student.Name) ? "Student" : student.Name.Trim();
            var baseName = Sanitize($"{roll}_{name}");
            var candidate = baseName;
            var suffix = 2;
            while (!usedNames.Add(candidate))
            {
                candidate = $"{baseName}_{suffix}";
                suffix++;
            }

            return candidate + ".pdf";
        }

        private static string Sanitize(string value)
        {
            var cleaned = Regex.Replace(value, @"[^\w\-. ]+", "_").Trim(' ', '.', '_');
            if (cleaned.Length > 80)
            {
                cleaned = cleaned[..80].Trim(' ', '.', '_');
            }

            return string.IsNullOrWhiteSpace(cleaned) ? "Student" : cleaned;
        }

        private static string Display(string? value) => value ?? string.Empty;

        // Same timing rule as Views/AdmitCard/ViewCard.cshtml.
        private static string ExamTiming(int examCentreId) =>
            examCentreId == 19 ? "11:30 AM to 01:30 PM" : "10 AM to 12 PM";

        private static IContainer LabelCell(IContainer container) =>
            container
                .Border(0.6f)
                .BorderColor(Colors.Grey.Darken2)
                .Background(Colors.Grey.Lighten2)
                .PaddingVertical(5)
                .PaddingHorizontal(6)
                .AlignMiddle();

        private static IContainer ValueCell(IContainer container) =>
            container
                .Border(0.6f)
                .BorderColor(Colors.Grey.Darken2)
                .PaddingVertical(5)
                .PaddingHorizontal(6)
                .AlignMiddle();

        private static byte[]? TryRead(string path) =>
            File.Exists(path) ? File.ReadAllBytes(path) : null;
    }
}
