using AllamaShibliQuiz.Models;
using AllamaShibliQuiz.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AllamaShibliQuiz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (TempData["AlertMessage"] is string msg)
            {
                ViewBag.AlertMessage = new AllamaShibliQuiz.Models.ViewModels.AlertMessageViewModel
                {
                    Type = TempData["AlertType"] as string ?? "Info",
                    Message = msg
                };
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}