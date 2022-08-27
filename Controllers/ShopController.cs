using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Controllers
{
    public class ShopController : BaseController
    {
        public IActionResult ViewProducts(int PageNumber = 1, int ResultsPerPage = 10, string SearchQuery = "", string CategoryID = "")
        {
            CategoryID = CategoryID.Decrypt();
            ResultsPerPage = ValidateResultsPerPage(ResultsPerPage);
            int AmountToSkip = (PageNumber - 1) * ResultsPerPage;
            int TotalItems = Product.Methods.GetTotalProducts(false);
            List<Product> Products = Product.Methods.GetProducts(ResultsPerPage, AmountToSkip, false, false, false, SearchQuery, CategoryID);

            if (!string.IsNullOrEmpty(CategoryID))
            {
                ProductCategory Category = new ProductCategory(CategoryID);
                ViewBag.CategoryName = Category.Description;
                ProductDepartment CurrentDepartment = new ProductDepartment(Category.DepartmentID);
                ViewBag.DepartmentName = CurrentDepartment.Description;
            }

            ViewBag.SearchQuery = SearchQuery;
            ViewBag.CategoryID = CategoryID;
            ViewBag.PageCount = CalculateNumberOfWholePages(TotalItems, ResultsPerPage);
            ViewBag.TotalItems = TotalItems;
            ViewBag.CurrentPage = PageNumber;
            ViewBag.ResultsPerPage = ResultsPerPage;
            ViewBag.ResultsPerPageSelection = GetResultsPerPageOptions(ResultsPerPage);
            ViewBag.DepartmentsAndCategories = ProductDepartment.Methods.GetProductDepartments(false);
            return View("~/Views/Shop/ProductCatalog.cshtml", Products);
        }

        [AutoValidateAntiforgeryToken]
        public IActionResult Search(IFormCollection Details, string CategoryID, int OldPageNumber, int ResultsPerPage)
        {
            int OldResultsPerPage = ResultsPerPage;
            //If the results per page changed, take the user back to page 1
            int PageNumber = ResultsPerPage != OldResultsPerPage ? 1 : OldPageNumber;
            string SearchQuery = Details["Search"].ToString();
            return RedirectToAction("ViewProducts", new { ResultsPerPage = ResultsPerPage, @PageNumber = PageNumber, @SearchQuery = SearchQuery, CategoryID = CategoryID });
        }

        public IActionResult ViewProduct(string ProductID)
        {
            ViewBag.ClientID = User.Identity.IsAuthenticated ? ClientID : null;
            Product Product = new Product(ProductID.Decrypt());
            return View("~/Views/Shop/Product.cshtml", Product);
        }

        [HttpGet]
        public IActionResult SearchProducts(string query)
        {
            List<Product> Result = new List<Product>();
            if(query.Length > 3)
            {
                Result = Product.Methods.GetProducts(5,0,false,false,false,query);
            }

            return PartialView("~/Views/Shop/_ProductSearch.cshtml", Result);
        }
    }
}