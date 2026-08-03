using Microsoft.AspNetCore.Mvc;

namespace Himam_main.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        // GET: /User/Home/Index  (الصفحة الرئيسية)
        public IActionResult Index()
        {
            return View();
        }

        // GET: /User/Home/About  (من نحن)
        public IActionResult About()
        {
            return View();
        }

        // GET: /User/Home/Contact  (تواصل معنا)
        public IActionResult Contact()
        {
            return View();
        }

        // GET: /User/Home/News  (أخبار وفعاليات)
        public IActionResult News()
        {
            return View();
        }

        // GET: /User/Home/NewsSingle/5  (تفاصيل خبر/فعالية واحدة)
        // TODO: لاحقاً يُستبدل بجلب الخبر الفعلي من قاعدة البيانات عبر id
        public IActionResult NewsSingle(int id)
        {
            return View();
        }
    }
}
