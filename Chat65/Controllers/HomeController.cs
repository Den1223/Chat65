using Microsoft.AspNetCore.Mvc;

namespace Chat65.Controllers
{
    public class HomeController : Controller
    {
        // Домашня сторінка
        public IActionResult Index()
        {
            return View();
        }

        // Сторінка чату
        public IActionResult Chat()
        {
            return View();
        }
    }
}