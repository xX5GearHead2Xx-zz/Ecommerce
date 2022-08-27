using Ecommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReviewController : BaseController
    {
        public IActionResult View(int ResultsPerPage = 10, int PageNumber = 1, Enums.ReviewStatus Status = Enums.ReviewStatus.Pending)
        {
            ResultsPerPage = ValidateResultsPerPage(ResultsPerPage);
            int AmountToSkip = (PageNumber - 1) * ResultsPerPage;
            int TotalItems = Review.Methods.GetTotalReviews(Status);
            List<Review> Reviews = Review.Methods.SearchReviews(ResultsPerPage, AmountToSkip, Status);

            ViewBag.PageCount = CalculateNumberOfWholePages(TotalItems, ResultsPerPage);
            ViewBag.TotalItems = TotalItems;
            ViewBag.CurrentPage = PageNumber;
            ViewBag.ResultsPerPage = ResultsPerPage;
            ViewBag.ResultsPerPageSelection = GetResultsPerPageOptions(ResultsPerPage);
            ViewBag.ReviewStatusOptions = Review.Methods.GetReviewStatusOptions((int)Status);
            return View("~/Views/Dashboard/Reviews.cshtml", Reviews);
        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult Search(IFormCollection Details, int OldResultsPerPage, int CurrentPage)
        {
            int ResultsPerPage = Convert.ToInt32(Details["ResultsPerPage"].ToString());
            //If the results per page changed, take the user back to page 1
            int PageNumber = ResultsPerPage != OldResultsPerPage ? 1 : CurrentPage;
            Enums.ReviewStatus ReviewStatus = (Enums.ReviewStatus)Convert.ToInt32(Details["ReviewStatus"].ToString());
            return RedirectToAction("View", new { ResultsPerPage = ResultsPerPage, PageNumber = PageNumber, Status = ReviewStatus });
        }

        public IActionResult ViewDetails(string ReviewID, string Success = "", string Error = "")
        {
            ViewBag.Success = Success;
            ViewBag.Error = Error;
            Review Review = new Review(ReviewID);
            ViewBag.OrderStatusOptions = Order.General.GetOrderStatusOptions((int)Enums.OrderStatus.NotSet);
            return View("~/Views/Dashboard/ReviewDetails.cshtml", Review);
        }

        public IActionResult RejectReview(string ReviewID)
        {
            Review Review = new Review(ReviewID.Decrypt());
            Review.Delete();
            return RedirectToAction("View");
        }

        public IActionResult ApproveReview(string ReviewID)
        {
            Review Review = new Review(ReviewID.Decrypt());
            Review.Delete();
            return RedirectToAction("View");
        }
    }
}
