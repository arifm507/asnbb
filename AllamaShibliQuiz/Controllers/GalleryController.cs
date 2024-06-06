using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AllamaShibliQuiz.Controllers
{
    public class GalleryController : Controller
    {
        // GET: GalleryController
        public ActionResult Index()
        {
            return View();
        }
    }
}
