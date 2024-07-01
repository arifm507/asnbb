using AllamaShibliQuiz.Data;
using AllamaShibliQuiz.Models;
using AllamaShibliQuiz.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult Index()
        {
            return View();
        }

        // POST: RegisterController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(StudentViewModel studentViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var student = _mapper.Map<Student>(studentViewModel);
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(RegisterSuccess), new { id = 1 });
                }
                ViewData["ValidateMessage"] = "Data is not correct";
                return View();
            }
            catch
            {
                return View();
            }
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
