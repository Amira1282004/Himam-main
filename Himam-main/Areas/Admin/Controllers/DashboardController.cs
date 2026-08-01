using Microsoft.AspNetCore.Mvc;

namespace Himam_main.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize] // فعّلها عند ربط نظام الحسابات الفعلي، بحيث لا يصل لهذه الصفحة إلا مستخدم مسجّل دخوله
    public class DashboardController : Controller
    {
        // GET: /Admin/Dashboard
        [HttpGet]
        public IActionResult Index()
        {
            // كل أقسام اللوحة (نظرة عامة، صفحات، مستخدمين، صلاحيات، مديرين، إعدادات، سجلات، تكاملات)
            // تُعرض داخل هذا الـ View نفسه، والتنقّل بينها يتم بالكامل عبر JS (dashNavigate)
            // دون إعادة تحميل الصفحة أو تغيير المسار — كما كان في النسخة الأصلية HTML.
            return View();
        }
    }
}