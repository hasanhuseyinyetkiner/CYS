using Microsoft.AspNetCore.Mvc;

namespace CYS.Controllers
{
    public class MobilHayvanController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MobilHayvanYonetimi()
        {
            return View();
        }
    }
}
