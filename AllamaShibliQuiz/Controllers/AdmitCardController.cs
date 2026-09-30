using AllamaShibliQuiz.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace AllamaShibliQuiz.Controllers
{
    public class AdmitCardController : Controller
    {
        private const string AdmitCardUnavailableMessage = "Admit cards will be available after the registration period closes on 31 Oct 2026. Please check back in November.";

        public IActionResult Index()
        {
            TempData["AlertType"] = "Info";
            TempData["AlertMessage"] = AdmitCardUnavailableMessage;
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult IndexAsync(AdmitCardRequestModel admitCardRequestModel)
        {
            TempData["AlertType"] = "Info";
            TempData["AlertMessage"] = AdmitCardUnavailableMessage;
            return RedirectToAction("Index", "Home");
        }

        public IActionResult GetCard(string searchInput)
        {
            TempData["AlertType"] = "Info";
            TempData["AlertMessage"] = AdmitCardUnavailableMessage;
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ViewCard(int Id)
        {
            TempData["AlertType"] = "Info";
            TempData["AlertMessage"] = AdmitCardUnavailableMessage;
            return RedirectToAction("Index", "Home");
        }
    }
}
