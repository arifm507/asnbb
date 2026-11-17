using System.Globalization;
using AllamaShibliQuiz.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AllamaShibliQuiz.Services
{
    public interface IAdmitCardPdfService
    {
        byte[] CreatePdf(IReadOnlyList<StudentViewModel> students);
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
            "Candidate must carry — i) ADMIT CARD \u00A0 ii) SCHOOL ID CARD \u00A0 iii) BALL POINT PEN (BLUE/BLACK)"
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

        public byte[] CreatePdf(IReadOnlyList<StudentViewModel> students)
        {
            return Document.Create(container =>
            {
                foreach (var student in students)
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(Mm(8));
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(style => style
                            .FontSize(10.5f)
                            .FontColor(Colors.Black)
                            .LineHeight(1.3f));
                        page.Content().Element(content => ComposeCard(content, student));
                    });
                }
            }).GeneratePdf();
        }

        private void ComposeCard(IContainer container, StudentViewModel student)
        {
            container
                .Border(2.5f, Colors.Black)
                .Padding(Mm(2.2f))
                .Border(1, Colors.Black)
                .PaddingHorizontal(Mm(5))
                .PaddingTop(Mm(4.5f))
                .PaddingBottom(Mm(4))
                .Column(column =>
                {
                    column.Item().Element(ComposeMasthead);
                    column.Item().PaddingTop(Mm(3)).Element(content => ComposeStudentTable(content, student));
                    column.Item().PaddingTop(Mm(3)).Element(content => ComposeExamTable(content, student));
                    column.Item().PaddingTop(Mm(3)).Element(ComposeSignatures);
                    column.Item().PaddingTop(Mm(3.2f)).Element(ComposeInstructions);
                });
        }

        private void ComposeMasthead(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    var logoSize = Mm(22);
                    row.ConstantItem(logoSize).Height(logoSize).AlignMiddle().AlignCenter()
                        .Element(cell => DrawImage(cell, _logo));

                    row.RelativeItem().AlignMiddle().Column(title =>
                    {
                        title.Item().AlignCenter()
                            .Text("Allama Shibli Nomani Brainiac Battle")
                            .Bold().FontSize(15.5f).LineHeight(1.15f);
                        title.Item().PaddingTop(Mm(0.8f)).AlignCenter()
                            .Text("Season - 3")
                            .Bold().FontSize(13);
                        title.Item().PaddingTop(Mm(0.6f)).AlignCenter()
                            .Text("Organized by - Team Brainiac Battle")
                            .FontSize(10).LineHeight(1.35f);
                        title.Item().PaddingTop(Mm(0.3f)).AlignCenter()
                            .Text("Sponsored by - Moral Public School, Chittepur, Saraimeer")
                            .FontSize(10).LineHeight(1.35f);
                        title.Item().PaddingTop(Mm(1.1f)).AlignCenter()
                            .Text("(Admit Card - 2026)")
                            .Bold().FontSize(10.5f).LetterSpacing(0.06f);
                    });

                    row.ConstantItem(logoSize);
                });

                column.Item().PaddingTop(Mm(2.6f)).LineHorizontal(2.5f).LineColor(Colors.Black);
                column.Item().PaddingTop(Mm(0.9f)).LineHorizontal(0.7f).LineColor(Colors.Black);
            });
        }

        private static void ComposeStudentTable(IContainer container, StudentViewModel student)
        {
            container.Table(table =>
            {
                DefineColumns(table);

                table.Cell().Element(LabelCell).Text("Name").Bold().FontSize(9);
                table.Cell().Element(ValueCell).Text(Upper(student.Name)).Bold();
                table.Cell().RowSpan(5).Element(ValueCell).AlignCenter().AlignMiddle().Element(PhotoBox);

                table.Cell().Element(LabelCell).Text("Roll Number").Bold().FontSize(9);
                table.Cell().Element(ValueCell).Text(Upper(student.RollNumber)).Bold().FontSize(13.5f).LetterSpacing(0.14f);

                table.Cell().Element(LabelCell).Text("Father's Name").Bold().FontSize(9);
                table.Cell().Element(ValueCell).Text(Upper(student.FatherName));

                table.Cell().Element(LabelCell).Text("Class").Bold().FontSize(9);
                table.Cell().Element(ValueCell).Text(student.Class.ToString());

                table.Cell().Element(LabelCell).Text("School Name").Bold().FontSize(9);
                table.Cell().Element(ValueCell).Text(Upper(student.SchoolName));
            });
        }

        private static void ComposeExamTable(IContainer container, StudentViewModel student)
        {
            container.Table(table =>
            {
                DefineColumns(table);

                table.Cell().ColumnSpan(3).Element(SectionCell).AlignCenter()
                    .Text("EXAM DAY DETAILS").Bold().FontSize(9.5f).LetterSpacing(0.16f);

                table.Cell().Element(LabelCell).Text("Date and Day of Exam").Bold().FontSize(9);
                table.Cell().Element(ValueCell).Text(ExamDateText(student.ExamDate)).Bold();
                table.Cell().RowSpan(4).Element(ValueCell).AlignBottom().Element(StudentSign);

                table.Cell().Element(LabelCell).Text("Exam Timing").Bold().FontSize(9);
                table.Cell().Element(ValueCell).Text(ExamTiming(student.ExamCentreId)).Bold();

                table.Cell().Element(LabelCell).Text("Center Code").Bold().FontSize(9);
                table.Cell().Element(ValueCell).Text(Upper(student.ExamCentreCode)).Bold();

                table.Cell().Element(LabelCell).Text("Exam Center").Bold().FontSize(9);
                table.Cell().Element(ValueCell).Text(Upper(student.ExamCentreName));
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

                table.Cell().Element(ValueCell).Height(Mm(24)).AlignCenter().AlignBottom()
                    .PaddingBottom(Mm(1)).Element(cell => DrawSignature(cell, _presidentSign));
                table.Cell().Element(ValueCell).Height(Mm(24)).AlignCenter().AlignBottom()
                    .PaddingBottom(Mm(1)).Element(cell => DrawSignature(cell, _controllerSign));

                table.Cell().Element(ValueCell).AlignCenter()
                    .Text("President Signature").Bold().FontSize(8.5f);
                table.Cell().Element(ValueCell).AlignCenter()
                    .Text("Examination Controller Signature").Bold().FontSize(8.5f);
            });
        }

        private static void ComposeInstructions(IContainer container)
        {
            container.Border(1, Colors.Black)
                .PaddingHorizontal(Mm(3.2f))
                .PaddingTop(Mm(2.4f))
                .PaddingBottom(Mm(2))
                .Column(notes =>
                {
                    notes.Item().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(Mm(1.2f))
                        .Text("Important Instructions for candidates:-").Bold().FontSize(10);

                    notes.Item().PaddingTop(Mm(1.6f)).Column(list =>
                    {
                        list.Spacing(Mm(0.7f));
                        for (var i = 0; i < Instructions.Length; i++)
                        {
                            var number = $"{i + 1}. ";
                            var line = Instructions[i];
                            list.Item().Text(text =>
                            {
                                text.DefaultTextStyle(style => style.FontSize(9).LineHeight(1.38f));
                                text.Span(number).Bold();
                                text.Span(line);
                            });
                        }
                    });
                });
        }

        private const string PhotoFrameSvg = """
            <svg xmlns="http://www.w3.org/2000/svg" width="35mm" height="45mm" viewBox="0 0 35 45">
              <rect x="0.35" y="0.35" width="34.3" height="44.3" fill="none" stroke="#111111" stroke-width="0.35" stroke-dasharray="1.1 0.7"/>
            </svg>
            """;

        private static void PhotoBox(IContainer container)
        {
            container.Width(Mm(35)).Height(Mm(45)).Layers(layers =>
            {
                layers.Layer().Svg(PhotoFrameSvg).FitArea();
                layers.PrimaryLayer().Padding(Mm(2)).AlignCenter().AlignMiddle()
                    .Text("Please affix your passport size photograph here.")
                    .FontSize(7.5f).LineHeight(1.35f).AlignCenter();
            });
        }

        private static void StudentSign(IContainer container)
        {
            container.PaddingHorizontal(Mm(1.8f)).PaddingBottom(Mm(1.4f)).Column(column =>
            {
                column.Item().LineHorizontal(1).LineColor(Colors.Black);
                column.Item().PaddingTop(Mm(1.2f)).AlignCenter()
                    .Text("Student Sign").Bold().FontSize(8);
            });
        }

        private static void DrawImage(IContainer container, byte[]? image)
        {
            if (image == null)
            {
                return;
            }

            container.Image(image).FitArea();
        }

        private static void DrawSignature(IContainer container, byte[]? image)
        {
            if (image == null)
            {
                return;
            }

            container.Height(Mm(18)).AlignCenter().Image(image).FitHeight();
        }

        private static void DefineColumns(TableDescriptor table)
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(Mm(46));
                columns.RelativeColumn();
                columns.ConstantColumn(Mm(42));
            });
        }

        private static IContainer LabelCell(IContainer container) =>
            container.Border(1, Colors.Black)
                .PaddingVertical(Mm(1.9f))
                .PaddingHorizontal(Mm(2.5f))
                .AlignMiddle();

        private static IContainer ValueCell(IContainer container) =>
            container.Border(1, Colors.Black)
                .PaddingVertical(Mm(1.9f))
                .PaddingHorizontal(Mm(2.5f))
                .AlignMiddle();

        private static IContainer SectionCell(IContainer container) =>
            container.Border(1, Colors.Black)
                .PaddingVertical(Mm(1.7f))
                .PaddingHorizontal(Mm(2))
                .AlignMiddle();

        private static string Upper(string? value) =>
            string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();

        // Same display as AdmitCardController: "22/11/2026 (Sunday)", then uppercased by the card.
        private static string ExamDateText(string? examDate)
        {
            if (string.IsNullOrWhiteSpace(examDate))
            {
                return string.Empty;
            }

            var trimmed = examDate.Trim();
            if (trimmed.Contains('('))
            {
                return trimmed.ToUpperInvariant();
            }

            if (!DateOnly.TryParseExact(trimmed, new[] { "dd/MM/yyyy", "yyyy-MM-dd" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return trimmed.ToUpperInvariant();
            }

            return $"{trimmed} ({date.DayOfWeek})".ToUpperInvariant();
        }

        // Same timing rule as Views/AdmitCard/ViewCard.cshtml.
        private static string ExamTiming(int examCentreId) =>
            examCentreId == 19 ? "11:30 AM TO 01:30 PM" : "10 AM TO 12 PM";

        private static float Mm(float millimetres) => millimetres * 72f / 25.4f;

        private static byte[]? TryRead(string path) =>
            File.Exists(path) ? File.ReadAllBytes(path) : null;
    }
}
