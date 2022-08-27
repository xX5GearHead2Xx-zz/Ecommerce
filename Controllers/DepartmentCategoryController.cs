using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentCategoryController : BaseController
    {
        public IActionResult ViewDepartments(bool Error, bool Success, string Message = "")
        {
            if (Error)
            {
                ViewBag.Error = Message;
            }

            if (Success)
            {
                ViewBag.Success = Message;
            }

            return View("~/Views/Dashboard/DepartmentsAndCategories.cshtml", GetDepartments());
        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult CreateDepartment(IFormCollection Details)
        {
            ProductDepartment Department = new ProductDepartment();
            Department.Description = Details["DepartmentName"].ToString();

            StringBuilder Message = new StringBuilder();

            if (Department.Save())
            {
                Message.Append("Department created successfully");
                return RedirectToAction("ViewDepartments", new { Message = Message, Success = true });
            }
            Message.Append("Department could not be created");
            return RedirectToAction("ViewDepartments", new { Message = Message, Error = true });
        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult CreateCategory(IFormCollection Details)
        {
            ProductCategory Category = new ProductCategory();
            Category.Description = Details["CategoryName"].ToString();
            Category.DepartmentID = Details["Department"].ToString();

            StringBuilder Message = new StringBuilder();

            if (Category.Save())
            {
                Message.Append("Category created successfully");
                return RedirectToAction("ViewDepartments", new { Message = Message, Success = true });
            }
            Message.Append("Category could not be created");
            return RedirectToAction("ViewDepartments", new { Message = Message, Error = true });
        }

        public List<ProductDepartment> GetDepartments()
        {
            return ProductDepartment.Methods.GetProductDepartments(true);
        }

        public IActionResult HideUnhideDepartment(string DepartmentID, bool MarkAsHidden)
        {
            ProductDepartment Department = new ProductDepartment(DepartmentID);
            StringBuilder Message = new StringBuilder();
            if (Department.HideUnhide(MarkAsHidden))
            {
                Message.Append("Department hidden successfully");
                return RedirectToAction("ViewDepartments", new { Message = Message, Success = true });
            }
            Message.Append("Department could not be hidden");
            return RedirectToAction("ViewDepartments", new { Message = Message, Error = true });
        }

        public IActionResult HideUnhideCategory(string CategoryID, bool MarkAsHidden)
        {
            ProductCategory Category = new ProductCategory(CategoryID);

            StringBuilder Message = new StringBuilder();
            if (Category.HideUnhide(MarkAsHidden))
            {
                Message.Append("Category hidden successfully");
                return RedirectToAction("ViewDepartments", new { Message = Message, Success = true });
            }
            Message.Append("Category could not be hidden");
            return RedirectToAction("ViewDepartments", new { Message = Message, Error = true });
        }
    }
}