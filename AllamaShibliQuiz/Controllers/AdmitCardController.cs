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
                var student = await _context.Students.Where(x => x.Status == 1 && (x.AadharNumber.Equals(admitCardRequestModel.SearchInput) 
                || x.MobileNumber.Equals(admitCardRequestModel.SearchInput))).AnyAsync();
                if (!student)
                {
                    ViewBag.AlertMessage = new AlertMessageViewModel()
                    {
                        Type = "Error",
                        Message = "Entered Mobile Number/Aadhar Number is not found. Please check the detail and try again."
                    };
                    return View(admitCardRequestModel);
                }
                return RedirectToAction(nameof(GetCard), new { SearchInput = admitCardRequestModel.SearchInput });
            }
            return View(admitCardRequestModel);
        }

        public async Task<ActionResult> GetCard(string searchInput)
        {
            if (string.IsNullOrEmpty(searchInput))
            {
                return RedirectToAction(nameof(Index));
            }
            var student = await _context.Students.Where(x => x.Status == 1 && (x.AadharNumber.Equals(searchInput) 
            || x.MobileNumber.Equals(searchInput))).ToListAsync();
            if (!student.Any())
            {
                return RedirectToAction(nameof(Index));
            }
            var studentViewModel = _mapper.Map<List<StudentViewModel>>(student);
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
