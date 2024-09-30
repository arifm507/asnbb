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
            //return Redirect("/");
            await LoadRegisterPageData();
            return View();
        }
        private async Task LoadRegisterPageData()
        {
            var schoolsList = await _context.Schools.Where(x => x.IsActive).OrderBy(x => x.Rank).Select(x => new SchoolViewModel { Id = x.Id, Name = x.Name, IsExamCentre = x.IsExamCentre, IsExternalExamCentre = x.IsExternalExamCentre }).ToListAsync();
            schoolsList.Add(new SchoolViewModel() { Id = 0, Name = "Other", IsExamCentre = false, IsExternalExamCentre = false });
            ViewBag.Schools = schoolsList;
            var examCentres = schoolsList.Where(x => x.IsExternalExamCentre).
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
                var isValid = await IsValidDataAsync(studentViewModel);
                studentViewModel.CreateDate = DateTime.Now;
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
        private async Task<bool> IsValidDataAsync(StudentViewModel studentViewModel)
        {
            var errorMessage = string.Empty;
            if (string.IsNullOrEmpty(studentViewModel.Gender))
            {
                errorMessage += "Please select Gender.";
            }
            if (studentViewModel.Class == 0)
            {
                errorMessage += "<br /> Please select Class.";
            }
            if (studentViewModel.MobileNumber.Length < 10)
            {
                errorMessage += "<br /> Please enter valid mobile number(10 digit).";
            }
            if (studentViewModel.AadharNumber.Length < 12)
            {
                errorMessage += "<br /> Please enter valid aadhar number(12 digit).";
            }
            if (studentViewModel.SchoolId == null)
            {
                errorMessage += "<br /> Please select school name.";
            }
            if (studentViewModel.ExamCentreId == 0)
            {
                errorMessage += "<br /> Please select the exam center.";
            }
            if (studentViewModel.SchoolId == 0 && string.IsNullOrEmpty(studentViewModel.OtherSchoolName))
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
            if (studentViewModel.SchoolId > 0)
            {
                var school = await GetSchoolAsync(studentViewModel.SchoolId.Value);
                studentViewModel.SchoolName = school.Name;
            }
            else
            {
                studentViewModel.SchoolName = studentViewModel.OtherSchoolName;
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
        [HttpGet]
        public ActionResult Verify()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> VerifyAsync(RegisterVerifyViewModel registerVerifyViewModel)
        {
            if (ModelState.IsValid)
            {
                var student = await _context.Students.Where(x => x.AadharNumber.Equals(registerVerifyViewModel.SearchInput) || x.MobileNumber.Equals(registerVerifyViewModel.SearchInput)).AnyAsync();
                if (!student)
                {
                    ViewBag.AlertMessage = new AlertMessageViewModel()
                    {
                        Type = "Error",
                        Message = "Entered Mobile Number/Aadhar Number is not found. Please check the detail and try again."
                    };
                    return View(registerVerifyViewModel);
                }
                return RedirectToAction(nameof(VerifySuccess), new { SearchInput = registerVerifyViewModel.SearchInput });
            }
            return View(registerVerifyViewModel);
        }
        public async Task<ActionResult> VerifySuccess(string searchInput)
        {
            if (string.IsNullOrEmpty(searchInput))
            {
                return RedirectToAction(nameof(Verify));
            }
            var student = await _context.Students.Where(x => x.AadharNumber.Equals(searchInput) || x.MobileNumber.Equals(searchInput)).ToListAsync();
            if (!student.Any())
            {
                return RedirectToAction(nameof(Verify));
            }
            var studentViewModel = _mapper.Map<List<StudentViewModel>>(student);
            return View(studentViewModel);
        }
        public async Task<SchoolViewModel> GetSchoolAsync(int id)
        {
            var school = await _context.Schools.FindAsync(id);
            return _mapper.Map<SchoolViewModel>(school);
        }
    }
}
