using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            List<IndexImage> IndexImages = IndexImage.Methods.GetIndexImages();
            ViewBag.FeaturedProducts = Product.Methods.GetProducts(10, 0, false, false, true);
            return View("~/Views/Home/Index.cshtml", IndexImages);
        }

        public IActionResult Contact(string Success = null, string Error = null)
        {
            ViewBag.Success = Success;
            ViewBag.Error = Error;
            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult SendContactEmail(IFormCollection Details)
        {
            ViewBag.Message = Details["Message"].ToString();
            List<string> Emails = new List<string>() { Details["Email"] };
            Emails.AddRange(CompanyEmails);
            if (SendEmail(Details["Subject"], RenderViewToStringAsync(this, "~/Views/EmailTemplates/Contact.cshtml"), Emails))
            {
                return RedirectToAction("Contact", new { Success = "Message sent successfully" });
            }
            else
            {
                return RedirectToAction("Contact", new { Error = "Message could not be sent, please retry" });
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult IndexManagement(string Success = "", string Error = "")
        {
            ViewBag.Success = Success;
            ViewBag.Error = Error;
            List<IndexImage> IndexImages = IndexImage.Methods.GetIndexImages();
            return View("~/Views/Dashboard/IndexManagement.cshtml", IndexImages);
        }

        [ValidateAntiForgeryToken, HttpPost, Authorize(Roles = "Admin")]
        public IActionResult AddIndexImage(IFormCollection Details, IFormFile Image)
        {
            IndexImage IndexImage = new IndexImage();
            using (var memoryStream = new MemoryStream())
            {
                Image.CopyTo(memoryStream);
                IndexImage.Data = memoryStream.ToArray();
            }
            IndexImage.URL = Details["URL"].ToString();
            if (IndexImage.Save())
            {
                return RedirectToAction("IndexManagement", new { Success = "Index Image saved successfully" });
            }
            return RedirectToAction("IndexManagement", new { Error = "Index image could not be saved" });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RemoveIndexImage(string IndexImageID)
        {
            IndexImage IndexImage = new IndexImage(IndexImageID);
            if (IndexImage.Delete())
            {
                return RedirectToAction("IndexManagement", new { Success = "Index Image removed successfully" });
            }
            return RedirectToAction("IndexManagement", new { Error = "Index Image could not be removed" });
        }
    }
}
