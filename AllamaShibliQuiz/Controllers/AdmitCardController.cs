using AllamaShibliQuiz.Data;
using AllamaShibliQuiz.Models.RequestModels;
using AllamaShibliQuiz.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllamaShibliQuiz.Controllers
{
    public class AdmitCardController : Controller
    {
        private readonly AsnbbDBContext _context;

        private readonly IMapper _mapper;
        //private readonly IConverter _converter;
        public AdmitCardController(AsnbbDBContext context, IMapper mapper)//, IConverter converter)
        {
            _context = context;
            _mapper = mapper;
            //_converter = converter;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> IndexAsync(AdmitCardRequestModel admitCardRequestModel)
        {
            if (ModelState.IsValid)
            {
                var found = true;
                var student = await _context.Students.Where(x => x.Status == 1 && (x.AadharNumber.Equals(admitCardRequestModel.SearchInput)
                || x.MobileNumber.Equals(admitCardRequestModel.SearchInput))).AnyAsync();
                if (!student)
                {
                    var school = await _context.Schools.Where(x => x.IsActive == true && x.ContactNumber == admitCardRequestModel.SearchInput).AnyAsync();
                    if (!school)
                    {
                        found = false;
                    }
                }
                if (found)
                {
                    return RedirectToAction(nameof(GetCard), new { SearchInput = admitCardRequestModel.SearchInput });
                }
                else
                {
                    ViewBag.AlertMessage = new AlertMessageViewModel()
                    {
                        Type = "Error",
                        Message = "Entered Mobile Number/Aadhar Number is not found. Please check the detail and try again."
                    };
                    return View(admitCardRequestModel);
                }
            }
            return View(admitCardRequestModel);
        }

        public async Task<ActionResult> GetCard(string searchInput)
        {
            if (string.IsNullOrEmpty(searchInput))
            {
                return RedirectToAction(nameof(Index));
            }
            var students = await _context.Students.Where(x => x.Status == 1 && (x.AadharNumber.Equals(searchInput)
            || x.MobileNumber.Equals(searchInput))).ToListAsync();
            if (!students.Any())
            {
                var school = await _context.Schools.Where(x => x.IsActive == true && x.ContactNumber == searchInput).FirstOrDefaultAsync();
                if (school != null)
                {
                    students = await _context.Students.Where(x => x.Status == 1 && x.SchoolId == school.Id && x.ExamCentreId == school.Id).OrderBy(x => x.Class).ThenBy(x => x.Name).ToListAsync();
                    if (!students.Any())
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            var studentViewModel = _mapper.Map<List<StudentViewModel>>(students);
            return View(studentViewModel);
        }
        public async Task<ActionResult> ViewCard(int Id)
        {
            var student = await _context.Students.Where(x => x.Status == 1 && x.Id == Id).FirstOrDefaultAsync();
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var studentData = _mapper.Map<StudentViewModel>(student);
            var center = await _context.Schools.Where(x => x.Id == student.ExamCentreId).Select(x => new { x.Name, x.CentreCode, x.ExamDate }).FirstOrDefaultAsync();
            studentData.ExamCentreName = center?.Name;
            studentData.ExamCentreCode = center?.CentreCode.ToString().PadLeft(2, '0');
            studentData.ExamDate = center?.ExamDate;
            return View(studentData);
        }
    }
}
