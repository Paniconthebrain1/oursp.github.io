using Microsoft.AspNetCore.Mvc;

namespace OurSunday.Areas.Admin.Controllers
{
    public class PostController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
