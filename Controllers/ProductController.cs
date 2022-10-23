using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : BaseController
    {
        public IActionResult ViewProducts(int ResultsPerPage = 10, int PageNumber = 1, string SearchQuery = "")
        {
            ResultsPerPage = ValidateResultsPerPage(ResultsPerPage);
            int AmountToSkip = (PageNumber - 1) * ResultsPerPage;
            int TotalItems = Product.Methods.GetTotalProducts(true);
            List<Product> Products = Product.Methods.GetProducts(ResultsPerPage, AmountToSkip, true, true, false, SearchQuery);

            ViewBag.PageCount = CalculateNumberOfWholePages(TotalItems, ResultsPerPage);
            ViewBag.TotalItems = TotalItems;
            ViewBag.CurrentPage = PageNumber;
            ViewBag.ResultsPerPage = ResultsPerPage;
            ViewBag.ResultsPerPageSelection = GetResultsPerPageOptions(ResultsPerPage);
            ViewBag.SearchQuery = SearchQuery;
            return View("~/Views/Dashboard/Products.cshtml", Products);
        }

        [AutoValidateAntiforgeryToken]
        public IActionResult Search(IFormCollection Details)
        {
            int ResultsPerPage = Convert.ToInt32(Details["ResultsPerPage"].ToString());
            int OldResultsPerPage = Convert.ToInt32(Details["HiddenResultsPerPage"].ToString());
            //If the results per page changed, take the user back to page 1
            int PageNumber = ResultsPerPage != OldResultsPerPage ? 1 : Convert.ToInt32(Details["HiddenPageNumber"].ToString());
            string SearchQuery = Details["Search"].ToString();
            return RedirectToAction("ViewProducts", new { ResultsPerPage = ResultsPerPage, @PageNumber = PageNumber, @SearchQuery = SearchQuery });
        }

        public IActionResult CreateProduct()
        {
            ViewBag.Departments = ProductDepartment.Methods.GetProductDepartments(false);
            return View("~/Views/Dashboard/CreateProduct.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult CreateProduct(IFormCollection Details, IFormFile Image1, IFormFile Image2, IFormFile Image3, IFormFile Image4)
        {
            try
            {
                List<ProductImage> CreatedImages = new List<ProductImage>();
                List<ProductSpecification> CreatedSpecifications = new List<ProductSpecification>();

                Product Product = new Product()
                {
                    CategoryID = Details["Category"].ToString(),
                    Name = Details["Name"].ToString(),
                    Description = Details["Description"].ToString(),
                    Price = Convert.ToInt32(Details["Price"].ToString()),
                    Quantity = Convert.ToInt32(Details["Quantity"].ToString()),
                    Brand = Details["Brand"].ToString(),
                    Color = Details["Color"].ToString(),
                    Weight = Convert.ToInt32(Details["Weight"].ToString()),
                    Height = Convert.ToInt32(Details["Height"].ToString()),
                    Width = Convert.ToInt32(Details["Width"].ToString()),
                    Depth = Convert.ToInt32(Details["Depth"].ToString()),
                    Featured = Convert.ToBoolean(Convert.ToInt32(Details["Featured"].ToString())),
                };

                StringBuilder SaveErrors = new StringBuilder();
                bool SaveError = false;

                if (Product.Save())
                {
                    bool SpecificationSaveError = false;
                    for (int SpecificationCount = Convert.ToInt32(Details["SpecificationCount"].ToString()); SpecificationCount > 0; SpecificationCount--)
                    {
                        ProductSpecification Spec = new ProductSpecification()
                        {
                            Value = Details["SpecificationValue_" + SpecificationCount].ToString(),
                            Description = Details["SpecificationName_" + SpecificationCount].ToString(),
                            ProductID = Product.Key,
                        };
                        if (Spec.Save())
                        {
                            CreatedSpecifications.Add(Spec);
                        }
                        else
                        {
                            SpecificationSaveError = true;
                        }
                    }

                    if (SpecificationSaveError)
                    {
                        SaveError = true;
                        SaveErrors.Append("Product specifications could not be saved <br/>");
                    }

                    List<ProductImage> ImagesToSave = new List<ProductImage>();
                    if (Image1 != null)
                    {
                        ProductImage Image = new ProductImage();
                        using (var memoryStream = new MemoryStream())
                        {
                            Image1.CopyTo(memoryStream);
                            Image.Data = memoryStream.ToArray();
                            Image.Order = 1;
                            ImagesToSave.Add(Image);
                        }
                    }

                    if (Image2 != null)
                    {
                        ProductImage Image = new ProductImage();
                        using (var memoryStream = new MemoryStream())
                        {
                            Image2.CopyTo(memoryStream);
                            Image.Order = 2;
                            Image.Data = memoryStream.ToArray();
                            ImagesToSave.Add(Image);
                        }
                    }

                    if (Image3 != null)
                    {
                        ProductImage Image = new ProductImage();
                        using (var memoryStream = new MemoryStream())
                        {
                            Image3.CopyTo(memoryStream);
                            Image.Order = 3;
                            Image.Data = memoryStream.ToArray();
                            ImagesToSave.Add(Image);
                        }
                    }

                    if (Image4 != null)
                    {
                        ProductImage Image = new ProductImage();
                        using (var memoryStream = new MemoryStream())
                        {
                            Image4.CopyTo(memoryStream);
                            Image.Order = 4;
                            Image.Data = memoryStream.ToArray();
                            ImagesToSave.Add(Image);
                        }
                    }

                    foreach (ProductImage Image in ImagesToSave)
                    {
                        Image.ProductID = Product.Key;
                        Image.Save();
                    }
                }
                else
                {
                    SaveError = true;
                    SaveErrors.Append("Product could not be saved");
                }

                if (SaveError)
                {
                    return RedirectToAction("EditProduct", new { ProductID = Product.Key, Error = SaveErrors.ToString() });
                }
                else
                {
                    return RedirectToAction("EditProduct", new { ProductID = Product.Key, Success = "Product created successfully" });
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("ProductController > CreateProduct " + Ex.Message);
            }
        }

        public IActionResult EditProduct(string ProductID, string Error = null, string Success = null)
        {
            ViewBag.Success = Success;
            ViewBag.Error = Error;
            ViewBag.Departments = ProductDepartment.Methods.GetProductDepartments(false);
            Product Product = new Product(ProductID);
            return View("~/Views/Dashboard/EditProduct.cshtml", Product);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditProduct(IFormCollection Details, IFormFile Image1, IFormFile Image2, IFormFile Image3, IFormFile Image4)
        {
            string ProductID = Details["ProductID"].ToString();

            try
            {
                Product ExistingProduct = new Product(ProductID);
                ExistingProduct.CategoryID = Details["Category"].ToString();
                ExistingProduct.Name = Details["Name"].ToString();
                ExistingProduct.Description = Details["Description"].ToString();
                ExistingProduct.Price = Convert.ToInt32(Details["Price"].ToString());
                ExistingProduct.Quantity = Convert.ToInt32(Details["Quantity"].ToString());
                ExistingProduct.Brand = Details["Brand"].ToString();
                ExistingProduct.Color = Details["Color"].ToString();
                ExistingProduct.Weight = Convert.ToInt32(Details["Weight"].ToString());
                ExistingProduct.Height = Convert.ToInt32(Details["Height"].ToString());
                ExistingProduct.Width = Convert.ToInt32(Details["Width"].ToString());
                ExistingProduct.Depth = Convert.ToInt32(Details["Depth"].ToString());
                ExistingProduct.Featured = Convert.ToBoolean(Convert.ToInt32(Details["Featured"].ToString()));

                if (ExistingProduct.Save())
                {
                    //Update or create product specifications
                    for (int Counter = 0; Counter <= Convert.ToInt32(Details["SpecificationCount"].ToString()); Counter++)
                    {
                        ProductSpecification Specification;
                        if (!string.IsNullOrEmpty(Details["ProductSpecificationID_" + Counter].ToString()))
                        {
                            Specification = new ProductSpecification(Details["ProductSpecificationID_" + Counter].ToString());
                            Specification.Value = Details["SpecificationValue_" + Counter].ToString();
                            Specification.Description = Details["SpecificationName_" + Counter].ToString();
                        }
                        else
                        {
                            Specification = new ProductSpecification()
                            {
                                Value = Details["SpecificationValue_" + Counter].ToString(),
                                Description = Details["SpecificationName_" + Counter].ToString(),
                                ProductID = ExistingProduct.Key,
                            };
                        }
                        Specification.Save();
                    }

                    //Delete any specifications that were removed
                    if (!string.IsNullOrEmpty(Details["RemovedSpecifications"].ToString()))
                    {
                        string[] RemovedSpecification = Details["RemovedSpecifications"].ToString().Split(',');
                        foreach (string RemovedSpec in RemovedSpecification)
                        {
                            ProductSpecification specification = new ProductSpecification(RemovedSpec);
                            specification.Delete();
                        }
                    }

                    //Delete any images that were removed
                    if (!string.IsNullOrEmpty(Details["RemovedImages"].ToString()))
                    {
                        string[] RemovedImages = Details["RemovedImages"].ToString().Split(',');
                        foreach (string RemovedImg in RemovedImages)
                        {
                            ProductImage image = new ProductImage(RemovedImg);
                            image.Delete();
                        }
                    }

                    List<ProductImage> ImagesToSave = new List<ProductImage>();
                    if (Image1 != null)
                    {
                        ProductImage Image;
                        if (!string.IsNullOrEmpty(Details["Image1ID"].ToString()))
                        {
                            //The image exists, but the data changed so we need to update the image source
                            Image = new ProductImage(Details["Image1ID"].ToString());
                        }
                        else
                        {
                            Image = new ProductImage();
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            Image1.CopyTo(memoryStream);
                            Image.Data = memoryStream.ToArray();
                            Image.Order = 1;
                            ImagesToSave.Add(Image);
                        }
                    }

                    if (Image2 != null)
                    {
                        ProductImage Image;
                        if (!string.IsNullOrEmpty(Details["Image2ID"].ToString()))
                        {
                            //The image exists, but the data changed so we need to update the image source
                            Image = new ProductImage(Details["Image2ID"].ToString());
                        }
                        else
                        {
                            Image = new ProductImage();
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            Image2.CopyTo(memoryStream);
                            Image.Order = 2;
                            Image.Data = memoryStream.ToArray();
                            ImagesToSave.Add(Image);
                        }
                    }

                    if (Image3 != null)
                    {
                        ProductImage Image;
                        if (!string.IsNullOrEmpty(Details["Image3ID"].ToString()))
                        {
                            //The image exists, but the data changed so we need to update the image source
                            Image = new ProductImage(Details["Image3ID"].ToString());
                        }
                        else
                        {
                            Image = new ProductImage();
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            Image3.CopyTo(memoryStream);
                            Image.Order = 3;
                            Image.Data = memoryStream.ToArray();
                            ImagesToSave.Add(Image);
                        }
                    }

                    if (Image4 != null)
                    {
                        ProductImage Image;
                        if (!string.IsNullOrEmpty(Details["Image4ID"].ToString()))
                        {
                            //The image exists, but the data changed so we need to update the image source
                            Image = new ProductImage(Details["Image4ID"].ToString());
                        }
                        else
                        {
                            Image = new ProductImage();
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            Image4.CopyTo(memoryStream);
                            Image.Order = 4;
                            Image.Data = memoryStream.ToArray();
                            ImagesToSave.Add(Image);
                        }
                    }

                    foreach (ProductImage Image in ImagesToSave)
                    {
                        Image.ProductID = ExistingProduct.Key;
                        Image.Save();
                    }

                    return RedirectToAction("EditProduct", new { @ProductID = ExistingProduct.Key, @Success = "The product details have been updated" });
                }
                else
                {
                    return RedirectToAction("EditProduct", new { @ProductID = ExistingProduct.Key, @Success = "The product could not be updated" });
                }
            }
            catch (Exception Ex)
            {
                return RedirectToAction("EditProduct", new { @ProductID = ProductID, @Error = "Something went wrong while updating the product " + Ex.Message });
            }
        }

        public IActionResult HideUnhide(string ProductID, bool MarkAsHidden)
        {
            Product Product = new Product(ProductID);
            Product.HideUnhide(MarkAsHidden);
            return RedirectToAction("ViewProducts");
        }
    }
}