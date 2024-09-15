using AllamaShibliQuiz.Data;
using AllamaShibliQuiz.Helpers;
using AllamaShibliQuiz.Models;
using AllamaShibliQuiz.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AllamaShibliQuiz.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AsnbbDBContext _context;
        private readonly IMapper _mapper;
        private int validFileSize = 200; //200 KB
        public AdminController(AsnbbDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                ViewBag.AlertMessage = new AlertMessageViewModel()
                {
                    Type = "Error",
                    Message = "Please enter valid Email or Password."
                };
                return View();
            }
            var encryptedPwd = login.Password.EncryptString();
            var loginUser = await _context.AdminUsers.AsNoTracking().Where(x => x.Email == login.Email && x.Password == encryptedPwd).FirstOrDefaultAsync();
            if (loginUser != null)
            {
                List<Claim> claims = new List<Claim>() {
                                        new Claim(ClaimTypes.NameIdentifier, login.Email),
                                        new Claim(ClaimTypes.Email, login.Email),
                                        new Claim(ClaimTypes.Role, "Admin")
                    };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);

                return RedirectToAction("Dashboard");
            }
            ViewBag.AlertMessage = new AlertMessageViewModel()
            {
                Type = "Error",
                Message = "Email or Password is incorrect"
            };
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        #region "Student"
        public async Task<IActionResult> Dashboard()
        {
            var students = await _context.Students.AsNoTracking().OrderByDescending(x => x.CreateDate).OrderBy(x => x.Status).ToListAsync();
            var studentsView = _mapper.Map<List<StudentViewModel>>(students);
            var totalStudents = studentsView.Count();
            var totalStudentsApproved = studentsView.Where(x => x.Status == 1).Count();
            var totalStudentsPending = studentsView.Where(x => x.Status == 0).Count();
            var totalStudentsRejected = studentsView.Where(x => x.Status == 2).Count();
            var dashboardData = new DashboardViewModel()
            {
                Students = studentsView,
                TotalStudentRegistered = totalStudents,
                TotalStudentApproved = totalStudentsApproved,
                TotalStudentPending = totalStudentsPending,
                TotalStudentRejected = totalStudentsRejected
            };
            return View(dashboardData);
        }
        public async Task<IActionResult> StudentDetails(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            var studentView = _mapper.Map<StudentViewModel>(student);
            return PartialView("_StudentDetails", studentView);
        }
        public async Task<IActionResult> Approve(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            student.Status = 1;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }
        public async Task<IActionResult> Reject(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            student.Status = 2;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }
        #endregion


        #region "Team Members"
        public async Task<IActionResult> Team()
        {
            var teams = await _context.Teams.AsNoTracking().ToListAsync();
            var teamsView = _mapper.Map<List<TeamViewModel>>(teams);
            return View(teamsView);
        }

        public async Task<IActionResult> TeamAddOrEdit(int? id)
        {
            if (id != null)
            {
                var teamMember = await _context.Teams.FindAsync(id);
                var teamsView = _mapper.Map<TeamViewModel>(teamMember);
                if (teamsView != null)
                {
                    return View("TeamAddOrEdit", teamsView);
                }
            }
            return View("TeamAddOrEdit");
        }

        [HttpPost]
        public async Task<IActionResult> TeamAddOrEdit(TeamViewModel teamModel, IFormFile image)
        {
            var team = new Team();
            if (teamModel.Id == 0)
            {
                team = _mapper.Map<Team>(teamModel);
            }
            else
            {
                team = await _context.Teams.FindAsync(teamModel.Id);
            }
            if (team == null)
            {
                ViewData["ValidateMessage"] = "Can not add or update Team Member";
                return View("TeamAddOrEdit");
            }
            var isValid = IsValidData(teamModel);
            if (!isValid)
            {
                return View("TeamAddOrEdit");
            }
            if (image != null && image.Length > 0)
            {
                var fileSize = image.Length/1024;
                if (fileSize > validFileSize) {
                    ViewData["ValidateMessage"] = "Please upload file size less than 200 KB.";
                    return View("TeamAddOrEdit");
                }
                using (var ms = new MemoryStream())
                {
                    await image.CopyToAsync(ms);
                    team.Image = ms.ToArray();
                    team.ImageName = image.FileName;
                    team.ContentType = image.ContentType;
                }
            }

            if (teamModel.Id == 0)
            {
                _context.Teams.Add(team);
            }
            else
            {
                team.Role = teamModel.Role;
                team.FullName = teamModel.FullName;
                team.FatherName = teamModel.FatherName;
                team.MobileNumber = teamModel.MobileNumber;
                team.AadharNumber = teamModel.AadharNumber;
                team.EmailId = teamModel.EmailId;
                team.PermanentAddress = teamModel.PermanentAddress;
                team.WhatsappNumber = teamModel.WhatsappNumber;
                team.HigherQualification = teamModel.HigherQualification;
                team.AdditionalQualification = teamModel.AdditionalQualification;
                team.OtherSkills = teamModel.OtherSkills;
                team.TeachingExperience = teamModel.TeachingExperience;
                team.CurrentSchoolName = teamModel.CurrentSchoolName;
                team.JoiningDate = teamModel.JoiningDate;
                _context.Teams.Update(team);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Team");
        }

        private bool IsValidData(TeamViewModel teamMember)
        {
            var errorMessage = string.Empty;
            if (teamMember.Role == "0")
            {
                errorMessage += "Please select Role.";
            }
            if (teamMember.MobileNumber.Length < 10)
            {
                errorMessage += "<br /> Please enter valid mobile number(10 digit).";
            }
            if (teamMember.WhatsappNumber.Length < 10)
            {
                errorMessage += "<br /> Please enter valid whatsapp number(10 digit).";
            }
            if (teamMember.AadharNumber.Length < 12)
            {
                errorMessage += "<br /> Please enter valid aadhar number(12 digit).";
            }
            if (teamMember.JoiningDate == DateTime.MinValue || teamMember.JoiningDate > DateTime.Now)
            {
                errorMessage += "<br /> Please enter valid joining date.";
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

        public async Task<IActionResult> TeamMemberDetails(int id)
        {
            var teamMember = await _context.Teams.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }
            var teamMemberView = _mapper.Map<TeamViewModel>(teamMember);
            return PartialView("_TeamMemberDetails", teamMemberView);
        }

        public async Task<IActionResult> DeleteTeamMember(int id)
        {
            var teamMember = await _context.Teams.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }
            _context.Teams.Remove(teamMember);
            await _context.SaveChangesAsync();
            return RedirectToAction("Team");
        }
        #endregion
    }
}
