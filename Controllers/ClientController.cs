using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Controllers
{
    public class ClientController : BaseController
    {
        [Authorize(Roles = "Admin")]
        public IActionResult View(int ResultsPerPage = 10, int PageNumber = 1, string Email = "")
        {
            ResultsPerPage = ValidateResultsPerPage(ResultsPerPage);
            int AmountToSkip = (PageNumber - 1) * ResultsPerPage;
            int TotalItems = Client.Methods.GetTotalClients();
            List<Client> Clients = Client.Methods.GetClients(ResultsPerPage, AmountToSkip, Email);
            if (!string.IsNullOrEmpty(Email))
            {
                TotalItems = Clients.Count();
            }
            
            ViewBag.PageCount = CalculateNumberOfWholePages(TotalItems, ResultsPerPage);
            ViewBag.TotalItems = TotalItems;
            ViewBag.CurrentPage = PageNumber;
            ViewBag.ResultsPerPage = ResultsPerPage;
            ViewBag.ResultsPerPageSelection = GetResultsPerPageOptions(ResultsPerPage);
            ViewBag.Email = Email;
            return View("~/Views/Dashboard/Clients.cshtml", Clients);
        }

        [AutoValidateAntiforgeryToken, Authorize(Roles = "Admin")]
        public IActionResult Search(IFormCollection Details)
        {
            int ResultsPerPage = Convert.ToInt32(Details["ResultsPerPage"].ToString());
            int OldResultsPerPage = Convert.ToInt32(Details["HiddenResultsPerPage"].ToString());
            //If the results per page changed, take the user back to page 1
            int PageNumber = ResultsPerPage != OldResultsPerPage ? 1 : Convert.ToInt32(Details["HiddenPageNumber"].ToString());
            string SearchQuery = Details["Email"].ToString();
            return RedirectToAction("View", new { ResultsPerPage = ResultsPerPage, @PageNumber = PageNumber, @Email = SearchQuery });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ViewDetails(string ClientID)
        {
            Client Client = new Client(ClientID);
            return View("~/Views/Dashboard/ClientDetails.cshtml", Client);
        }
    }
}