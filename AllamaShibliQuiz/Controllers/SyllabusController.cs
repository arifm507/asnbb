using AllamaShibliQuiz.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AllamaShibliQuiz.Controllers
{
    public class SyllabusController : Controller
    {
        private IWebHostEnvironment Environment;

        public SyllabusController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }

        // GET: SyllabusController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SyllabusViewModel result)
        {
            if (ModelState.IsValid)
            {
                if (result.Class > 0)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, $"syllabus/class-{result.Class}.jpeg");
                    if (System.IO.File.Exists(path))
                    {
                        return File(System.IO.File.OpenRead(path), "image/jpeg", Path.GetFileName(path));
                    }

                }
            }
            return View(result);
        }
    }
}
