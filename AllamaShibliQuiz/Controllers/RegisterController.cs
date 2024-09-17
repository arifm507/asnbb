using AllamaShibliQuiz.Data;
using AllamaShibliQuiz.Models;
using AllamaShibliQuiz.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AllamaShibliQuiz.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AsnbbDBContext _context;

        private readonly IMapper _mapper;
        public RegisterController(AsnbbDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: RegisterController
        public async Task<ActionResult> IndexAsync()
        {
            await LoadRegisterPageData();
            return View();
        }
        private async Task LoadRegisterPageData()
        {
            var schoolsList = await _context.Schools.Where(x => x.IsActive).Select(x => new SchoolViewModel { Id = x.Id, Name = x.Name, IsExamCentre = x.IsExamCentre }).ToListAsync();
            schoolsList.Add(new SchoolViewModel() { Id = 0, Name = "Other", IsExamCentre = false });
            ViewBag.Schools = schoolsList;
            var examCentres = schoolsList.Where(x => x.IsExamCentre).
                Select(x => new { x.Id, x.Name }).ToList();
            ViewBag.ExamCentres = examCentres;
        }

        // POST: RegisterController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(StudentViewModel studentViewModel)
        {
            try
            {
                var isValid = IsValidData(studentViewModel);
                await LoadRegisterPageData();
                if (ModelState.IsValid)
                {
                    if (!isValid)
                    {
                        return View(studentViewModel);
                    }
                    if (await AlreadyRegistered(studentViewModel))
                    {
                        return View(studentViewModel);
                    }
                    if (studentViewModel.SchoolName == "Other")
                    {
                        studentViewModel.SchoolName = studentViewModel.OtherSchoolName;
                    }
                    studentViewModel.CreateDate = DateTime.Now;
                    var student = _mapper.Map<Student>(studentViewModel);
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(RegisterSuccess), new { id = student.Id });
                }
                return View(studentViewModel);
            }
            catch
            {
                return View(studentViewModel);
            }
        }
        private bool IsValidData(StudentViewModel student)
        {
            var errorMessage = string.Empty;
            if (student.Gender == "0")
            {
                errorMessage += "Please select Gender.";
            }
            if (student.Class == 0)
            {
                errorMessage += "<br /> Please select Class.";
            }
            if (student.MobileNumber.Length < 10)
            {
                errorMessage += "<br /> Please enter valid mobile number(10 digit).";
            }
            if (student.AadharNumber.Length < 12)
            {
                errorMessage += "<br /> Please enter valid aadhar number(12 digit).";
            }
            if (string.IsNullOrEmpty(student.SchoolName) || student.SchoolName == "0")
            {
                errorMessage += "<br /> Please Select the School.";
            }
            if (student.ExamCentre == 0)
            {
                errorMessage += "<br /> Please Select the Exam Center.";
            }
            if (student.SchoolName == "Other" && string.IsNullOrEmpty(student.OtherSchoolName))
            {
                errorMessage += "<br /> Please enter school name.";
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.AlertMessage = new AlertMessageViewModel()
                {
                    Type = "Error",
                    Message = errorMessage
                };
                return false;
            }
            return true;
        }
        private async Task<bool> AlreadyRegistered(StudentViewModel student)
        {
            var alreadyData = await _context.Students
                .Where(x => x.Name == student.Name
                        && x.FatherName == student.FatherName
                        && x.Class == student.Class
                        && x.Gender == student.Gender
                        && x.MobileNumber == student.MobileNumber
                        && x.Status <= 1)
                .AnyAsync();
            if (alreadyData)
            {
                ViewBag.AlertMessage = new AlertMessageViewModel()
                {
                    Type = "Error",
                    Message = $"Hi <b>{student.Name}</b>, you are already registered for ASNBB-2024. " +
                    $"Please verify your registration with admin team."
                };
            }
            return alreadyData;
        }
        public async Task<ActionResult> RegisterSuccess(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            var student = await _context.Students.FindAsync(id);
            var studentViewModel = _mapper.Map<StudentViewModel>(student);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(studentViewModel);
        }
    }
}
