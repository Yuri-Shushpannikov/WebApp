using Microsoft.AspNetCore.Mvc;

namespace WebApp.Backend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
