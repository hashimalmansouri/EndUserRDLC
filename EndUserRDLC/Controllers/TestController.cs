using Microsoft.AspNetCore.Mvc;

namespace EndUserRDLC.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
