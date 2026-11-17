using AllamaShibliQuiz.Data;
using AllamaShibliQuiz.Models.RequestModels;
using AllamaShibliQuiz.Models.ViewModels;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllamaShibliQuiz.Controllers
{
    public class AdmitCardController : Controller
    {
        private readonly AsnbbDBContext _context;
        private readonly AppMapper _mapper;

        public AdmitCardController(AsnbbDBContext context, AppMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AdmitCardRequestModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AdmitCardRequestModel admitCardRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return View(admitCardRequestModel);
            }

            var searchInput = admitCardRequestModel.SearchInput?.Trim();
            if (string.IsNullOrEmpty(searchInput))
            {
                return View(admitCardRequestModel);
            }

            var found = await _context.Students.AnyAsync(x => x.Status == 1 &&
                (x.AadharNumber == searchInput || x.MobileNumber == searchInput));
            if (!found)
            {
                found = await _context.Schools.AnyAsync(x => x.IsActive && x.ContactNumber == searchInput);
            }

            if (found)
            {
                return RedirectToAction(nameof(GetCard), new { searchInput });
            }

            ViewBag.AlertMessage = new AlertMessageViewModel
            {
                Type = "Error",
                Message = "Entered Mobile Number/Aadhar Number is not found. Please check the detail and try again."
            };
            return View(admitCardRequestModel);
        }

        public async Task<IActionResult> GetCard(string searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput))
            {
                return RedirectToAction(nameof(Index));
            }

            searchInput = searchInput.Trim();
            var students = await _context.Students
                .Where(x => x.Status == 1 && (x.AadharNumber == searchInput || x.MobileNumber == searchInput))
                .ToListAsync();

            if (students.Count == 0)
            {
                var school = await _context.Schools
                    .Where(x => x.IsActive && x.ContactNumber == searchInput)
                    .FirstOrDefaultAsync();
                if (school == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                students = await _context.Students
                    .Where(x => x.Status == 1 && x.SchoolId == school.Id && x.ExamCentreId == school.Id)
                    .OrderBy(x => x.Class)
                    .ThenBy(x => x.Name)
                    .ToListAsync();
                if (students.Count == 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            var studentViewModel = students.Select(_mapper.StudentToViewModel).ToList();
            return View(studentViewModel);
        }

        public async Task<IActionResult> ViewCard(int id)
        {
            var student = await _context.Students
                .Where(x => x.Status == 1 && x.Id == id)
                .FirstOrDefaultAsync();
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var studentData = _mapper.StudentToViewModel(student);
            var center = await _context.Schools
                .Where(x => x.Id == student.ExamCentreId)
                .Select(x => new { x.Name, x.CentreCode, x.ExamDate })
                .FirstOrDefaultAsync();
            studentData.ExamCentreName = center?.Name;
            studentData.ExamCentreCode = center?.CentreCode.ToString().PadLeft(2, '0');
            studentData.ExamDate = AppendExamDay(center?.ExamDate);
            return View(studentData);
        }

        // Turns "22/11/2026" into "22/11/2026 (Sunday)". Leaves the value untouched if it is not a valid date.
        private static string? AppendExamDay(string? examDate)
        {
            if (string.IsNullOrWhiteSpace(examDate))
            {
                return examDate;
            }

            var trimmed = examDate.Trim();
            if (!DateOnly.TryParseExact(trimmed, new[] { "dd/MM/yyyy", "yyyy-MM-dd" },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return examDate;
            }

            return $"{trimmed} ({date.DayOfWeek})";
        }
    }
}
