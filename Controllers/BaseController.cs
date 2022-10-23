using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using static Ecommerce.Models.Enums;

namespace Ecommerce.Controllers
{
    public partial class BaseController : Controller
    {
        public string ClientID
        {
            get
            {
                return User.Claims.Where(c => c.Type == "ClientID").First().Value;
            }
        }

        public List<string> CompanyEmails
        {
            get
            {
                return Startup.StaticConfiguration["EmailSettings:CompanyEmails"].Split(',').ToList();
            }
        }

        public class RecaptchaObject
        {
            public string success { get; set; }
        }

        public bool ValidateRecaptcha(string Captcha)
        {
            bool Valid = false;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
            (" https://www.google.com/recaptcha/api/siteverify?secret=" + Startup.StaticConfiguration["Recaptcha:RecaptchaPrivate"] + "&response=" + Captcha);
            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {

                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        RecaptchaObject data = JsonConvert.DeserializeObject<RecaptchaObject>(jsonResponse);// Deserialize Json

                        Valid = Convert.ToBoolean(data.success);
                    }
                }

                return Valid;
            }
            catch (Exception Ex)
            {
                throw new Exception("Ecommerce > baseController > ValiecdateRecaptcha " + Ex.Message);
            }
        }

        public int CalculateNumberOfWholePages(int ItemCount, int ResultsPerPage)
        {
            int NumberOfWholePages = 0;
            while (ItemCount >= ResultsPerPage)
            {
                NumberOfWholePages++;
                ItemCount -= ResultsPerPage;
            }
            //if there are remaining items, add an extra page
            if (ItemCount > 0)
            {
                NumberOfWholePages += 1;
            }

            return NumberOfWholePages;
        }

        public int ValidateResultsPerPage(int ResultsPerPage)
        {
            if (ResultsPerPage > 50)
            {
                return 50;
            }
            if (ResultsPerPage < 10)
            {
                return 10;
            }
            return ResultsPerPage;
        }

        public List<SelectListItem> GetResultsPerPageOptions(int SelectedOption = 0)
        {
            List<SelectListItem> Options = new List<SelectListItem>();
            Options.Add(new SelectListItem() { Value = "10", Text = "10", Selected = SelectedOption == 10 ? true : false });
            Options.Add(new SelectListItem() { Value = "25", Text = "25", Selected = SelectedOption == 25 ? true : false });
            Options.Add(new SelectListItem() { Value = "50", Text = "50", Selected = SelectedOption == 50 ? true : false });
            return Options;
        }

        public bool SendEmail(string Subject, string Message, List<string> Recipients)
        {
            try
            {
                string connectionString = Startup.StaticConfiguration["EmailSettings:ConnectionString"];
                EmailClient EmailClient = new EmailClient(connectionString);
                EmailContent emailContent = new EmailContent(Subject);
                emailContent.Html = Message;
                List<EmailAddress> emailAddresses = new List<EmailAddress>();
                foreach (string Address in Recipients)
                {
                    emailAddresses.Add(new EmailAddress(Address) { DisplayName = Address });
                }
                EmailRecipients emailRecipients = new EmailRecipients(emailAddresses);
                EmailMessage emailMessage = new EmailMessage(Startup.StaticConfiguration["EmailSettings:SiteEmail"], emailContent, emailRecipients);
                SendEmailResult emailResult = EmailClient.Send(emailMessage, CancellationToken.None);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string RenderViewToStringAsync(Controller controller, string viewNamePath)
        {
            if (string.IsNullOrEmpty(viewNamePath))
                viewNamePath = controller.ControllerContext.ActionDescriptor.ActionName;

            using (StringWriter writer = new StringWriter())
            {
                try
                {
                    IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                    ViewEngineResult viewResult = viewEngine.GetView(viewNamePath, viewNamePath, false);

                    ViewContext viewContext = new ViewContext(
                        controller.ControllerContext,
                        viewResult.View,
                        controller.ViewData,
                        controller.TempData,
                        writer,
                        new HtmlHelperOptions()
                    );

                    viewResult.View.RenderAsync(viewContext);

                    return writer.GetStringBuilder().ToString();
                }
                catch (Exception exc)
                {
                    return $"Failed - {exc.Message}";
                }
            }
        }

        public decimal CalculateVAT(decimal Amount)
        {
            return (Amount * 15) / 100;
        }
    }
}
