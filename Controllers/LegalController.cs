using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class LegalController : Controller
    {
        public IActionResult TermsAndConditions()
        {
            ViewBag.DontShowCookiePrompt = "DontShow";
            return View();
        }

        public IActionResult PrivacyPolicy()
        {
            ViewBag.DontShowCookiePrompt = "DontShow";
            return View();
        }

        public IActionResult CookiePolicy()
        {
            ViewBag.DontShowCookiePrompt = "DontShow";
            return View();
        }

        public IActionResult Disclaimer()
        {
            ViewBag.DontShowCookiePrompt = "DontShow";
            return View();
        }

        public IActionResult ReturnsPolicy()
        {
            ViewBag.DontShowCookiePrompt = "DontShow";
            return View();
        }
    }
}
