using AllamaShibliQuiz.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace AllamaShibliQuiz.Controllers
{
    public class AnswerKeyController : Controller
    {
        private IWebHostEnvironment Environment;

        public AnswerKeyController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AnswerKeyRequestModel result)
        {
            if (ModelState.IsValid)
            {
                if (result.Class != "0")
                {
                    string path = Path.Combine(this.Environment.WebRootPath, $"answer_key/answerkey_{result.BookletCode}_{result.Class}.pdf");
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
