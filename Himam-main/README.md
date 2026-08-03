# همم الحياة — دليل تشغيل النظام

موقع تعريفي إعلاني مع نظام CRM لإدارة المحتوى، مبني على **ASP.NET Core 10 MVC** و **SQL Server**.

---

## المتطلبات

| المكون | الإصدار |
|--------|---------|
| .NET SDK | 10.0+ |
| SQL Server | Express أو أعلى |
| قاعدة البيانات | `HimanAlhayah` (موجودة مسبقاً) |

---

## التشغيل المحلي

```powershell
cd Himam-main
dotnet restore
dotnet user-secrets set "SiteSettings:SuperAdmin:Password" "YOUR_PASSWORD"
dotnet run
```

| الرابط | الوصف |
|--------|-------|
| `http://localhost:5011` | الموقع العام |
| `http://localhost:5011/Admin/Account/Login` | لوحة التحكم |
| `http://localhost:5011/robots.txt` | Robots |
| `http://localhost:5011/sitemap.xml` | Sitemap |

**بيانات الدخول الافتراضية (Super Admin):**
- المستخدم: `Admin_Z07`
- البريد: `himamalhayah0@gmail.com`
- كلمة المرور: تُضبط عبر User Secrets (انظر أعلاه)

---

## خطة التنفيذ الكاملة

### المرحلة 1 — البنية التحتية ✅ (مكتملة جزئياً)
- [x] التحقق من .NET و SQL Server
- [x] تشغيل المشروع محلياً
- [x] نقل كلمة مرور المدير الأعلى إلى User Secrets
- [x] إعدادات `appsettings.json` (SMTP، SEO، Audit)
- [ ] نشر Production على IIS/Nginx + SSL

### المرحلة 2 — الأمان والصلاحيات ✅ (مكتملة جزئياً)
- [x] خدمة Audit Log (`AuditLogService`)
- [x] تسجيل الدخول/الخروج/المحاولات الفاشلة
- [x] سياسات Authorization حسب الأدوار الأربعة
- [x] Security Headers (HSTS, X-Frame-Options, CSP basics)
- [x] CSRF على النماذج
- [x] API السجلات الأمنية في لوحة التحكم
- [ ] Rate limiting لمحاولات الدخول
- [ ] قفل الحساب بعد محاولات فاشلة
- [ ] 2FA (اختياري)

### المرحلة 3 — Backend CRM ✅ (مكتملة جزئياً)
- [x] `ContentService` + APIs: Pages, News, Settings, Services, Users, Media
- [x] نموذج تواصل معنا → قاعدة البيانات
- [x] ربط JavaScript للوحة التحكم (حفظ الصفحات، الأخبار، الإعدادات، المستخدمين)
- [x] رفع الوسائط `/Admin/Api/Media` (صور/PDF/فيديو محدود)
- [ ] محرر أخبار غني (بدون prompts)
- [ ] إدارة الخدمات من واجهة لوحة التحكم
- [ ] دعوات المديرين بالبريد

### المرحلة 4 — ربط الواجهة العامة ✅ (مكتملة جزئياً)
- [x] الرئيسية + من نحن + تواصل من DB
- [x] صفحة الأخبار + تفاصيل الخبر
- [ ] قطاعات الخدمات ديناميكية في الرئيسية

### المرحلة 5 — SEO ✅ (مكتملة جزئياً)
- [x] `robots.txt` (يمنع فهرسة `/Admin/`)
- [x] `sitemap.xml` ديناميكي
- [x] Canonical URLs + hreflang
- [x] Meta title/description لكل صفحة
- [x] صفحة 404
- [x] وضع الصيانة (من الإعدادات)
- [ ] إعادة توجيه 301 عند تغيير الروابط
- [ ] Google Search Console (يتطلب تدخلك)
- [ ] Open Graph / Twitter Cards

### المرحلة 6 — البريد والدعوات ⚠️ (يتطلب تدخلك)
- [ ] إعداد SMTP في `appsettings.json` أو User Secrets
- [ ] إرسال رابط دعوة للمديرين الجدد
- [ ] إشعار بريدي عند رسالة تواصل جديدة
- [ ] تأكيد OTP/SMS للحسابات (يتطلب مزود SMS)

---

## الأدوار والصلاحيات

| الدور | الصلاحيات الرئيسية |
|-------|-------------------|
| **Super Admin** | كل شيء: المستخدمين، الصلاحيات، الإعدادات، السجلات |
| **Site Manager** | المحتوى، الخدمات، الفعاليات، التقارير، الرسائل |
| **Content Editor** | إنشاء/تعديل المحتوى، رفع صور، مسودات (النشر مشروط) |
| **Customer Service** | عرض رسائل التواصل، تحديث الحالة، ملاحظات |

---

## Audit Log

- **مدة الاحتفاظ:** 365 يوم (قابلة للتعديل في Settings)
- **يُسجّل:** دخول، خروج، محاولات فاشلة، رسائل تواصل، (لاحقاً: CRUD، تغيير صلاحيات)
- **لا يمكن للمستخدمين العاديين تعديله أو حذفه**

---

## ⚠️ خطوات تحتاج تدخلك

### 1. كلمة مرور المدير الأعلى
```powershell
dotnet user-secrets set "SiteSettings:SuperAdmin:Password" "كلمة_مرور_قوية"
```

### 2. إعداد البريد (SMTP) — لدعوات المديرين والإشعارات
```powershell
dotnet user-secrets set "SmtpSettings:Host" "smtp.gmail.com"
dotnet user-secrets set "SmtpSettings:Port" "587"
dotnet user-secrets set "SmtpSettings:Username" "your-email@gmail.com"
dotnet user-secrets set "SmtpSettings:Password" "app-password"
dotnet user-secrets set "SmtpSettings:FromEmail" "info@himamalhayah.sa"
```

### 3. SSL/HTTPS في Production
- احصل على شهادة SSL (Let's Encrypt أو من المزود)
- اضبط IIS أو Nginx كـ reverse proxy
- حدّث `SiteSettings:PublicBaseUrl` إلى `https://himamalhayah.sa`

### 4. Google Search Console
- سجّل الموقع في [Google Search Console](https://search.google.com/search-console)
- أرسل `sitemap.xml`
- تحقق من الملكية

### 5. OTP/SMS (إن رغبت)
- اختر مزود SMS (Twilio, Unifonic, etc.)
- زوّدنا بـ API Key و Sender ID

---

## هيكل المشروع

```
Himam-main/
├── Areas/
│   ├── User/          # الموقع العام
│   └── Admin/         # لوحة التحكم + API
├── Authorization/     # الأدوار والسياسات
├── Controllers/       # SEO (robots, sitemap)
├── Data/              # DbContext
├── Middleware/        # Security Headers
├── Models/            # 17 جدول
├── Services/          # Auth, Audit, Seeder
└── wwwroot/           # CSS, JS, Assets
```

---

## Connection String

```json
"Server=.\\SQLEXPRESS;Database=HimanAlhayah;Trusted_Connection=True;TrustServerCertificate=True"
```

عدّله في `appsettings.json` أو `appsettings.Development.json` حسب بيئتك.
