using AllamaShibliQuiz.Models;
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
                    string path = Path.Combine(this.Environment.WebRootPath, $"results/class_{result.Class}.pdf");
                    if (System.IO.File.Exists(path))
                    {
                        return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
                    }

                }
            }
            return View(result);
        }
        // GET: ResultController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ResultController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ResultController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ResultController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ResultController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ResultController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ResultController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
