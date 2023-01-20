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
        public IActionResult ViewDepartments(string Error = "", string Success = "")
        {
            ViewBag.Error = Error;
            ViewBag.Success = Success;
            List<ProductDepartment> Departments = ProductDepartment.Methods.GetProductDepartments(true);
            return View("~/Views/Dashboard/DepartmentsAndCategories.cshtml", Departments);
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

        public IActionResult EditDepartment(IFormCollection Details)
        {
            if (!string.IsNullOrEmpty(Details["EditDepartmentID"].ToString()))
            {
                ProductDepartment Department = new ProductDepartment(Details["EditDepartmentID"].ToString());
                //Make sure that the old department loaded
                if (!string.IsNullOrEmpty(Department.Description))
                {
                    Department.Description = Details["EditDepartmentName"].ToString();
                    Department.Save();

                    return RedirectToAction("ViewDepartments", new { Success = "Department updated successfully" });
                }
            }

            return RedirectToAction("ViewDepartments", new { Success = "Something went wrong while updating the department" });
        }

        public IActionResult EditCategory(IFormCollection Details)
        {

            if (!string.IsNullOrEmpty(Details["EditCategoryID"].ToString()))
            {
                ProductCategory Category = new ProductCategory(Details["EditCategoryID"].ToString());
                //Make sure that the old category loaded
                if (!string.IsNullOrEmpty(Category.Description))
                {
                    Category.Description = Details["EditCategoryName"].ToString();
                    Category.Save();

                    return RedirectToAction("ViewDepartments", new { Success = "Category updated successfully" });
                }
            }

            return RedirectToAction("ViewDepartments", new { Success = "Something went wrong while updating the category" });
        }
    }
}