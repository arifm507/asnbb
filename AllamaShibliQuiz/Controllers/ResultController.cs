using AllamaShibliQuiz.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AllamaShibliQuiz.Controllers
{
    public class ResultController : Controller
    {
        private IWebHostEnvironment Environment;

        public ResultController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }

        // GET: ResultController
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ResultViewModel result)
        {
            if (ModelState.IsValid)
            {
                if (result.Class > 0)
                {
                    string path = Path.Combine(this.Environment.WebRootPath, $"results/2024/result_{result.Class}.pdf");
                    if (System.IO.File.Exists(path))
                    {
                        return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
                    }

                }
            }
            return View(result);
        }
    }
}
