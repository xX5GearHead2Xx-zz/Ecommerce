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
    [Authorize(Roles = "Admin")]
    public class OrderController : BaseController
    {
        public IActionResult View(int ResultsPerPage = 10, int PageNumber = 1, Enums.OrderStatus OrderStatus = Enums.OrderStatus.Payed, string OrderNumber = "")
        {
            ResultsPerPage = ValidateResultsPerPage(ResultsPerPage);
            int AmountToSkip = (PageNumber - 1) * ResultsPerPage;
            int TotalItems = Order.General.GetTotalOrders(OrderStatus);
            List<Order> Orders = Order.General.GetOrders(ResultsPerPage, AmountToSkip, OrderStatus, OrderNumber);

            ViewBag.PageCount = CalculateNumberOfWholePages(TotalItems, ResultsPerPage);
            ViewBag.TotalItems = TotalItems;
            ViewBag.CurrentPage = PageNumber;
            ViewBag.ResultsPerPage = ResultsPerPage;
            ViewBag.ResultsPerPageSelection = GetResultsPerPageOptions(ResultsPerPage);
            ViewBag.OrderStatusOptions = Order.General.GetOrderStatusOptions((int)OrderStatus);
            ViewBag.OrderStatus = OrderStatus;
            ViewBag.OrderNumber = OrderNumber;
            return View("~/Views/Dashboard/Orders.cshtml", Orders);
        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult Search(IFormCollection Details)
        {
            int ResultsPerPage = Convert.ToInt32(Details["ResultsPerPage"].ToString());
            int OldResultsPerPage = Convert.ToInt32(Details["HiddenResultsPerPage"].ToString());
            //If the results per page changed, take the user back to page 1
            int PageNumber = ResultsPerPage != OldResultsPerPage ? 1 : Convert.ToInt32(Details["HiddenPageNumber"].ToString());
            string OrderNumber = Details["OrderNumber"].ToString();
            Enums.OrderStatus OrderStatus = (Enums.OrderStatus)Convert.ToInt32(Details["OrderStatus"].ToString());
            return RedirectToAction("View", new { ResultsPerPage = ResultsPerPage, PageNumber = PageNumber, OrderStatus = OrderStatus, OrderNumber = OrderNumber });
        }

        public IActionResult ViewDetails(string OrderID, string Success = "", string Error = "")
        {
            ViewBag.Success = Success;
            ViewBag.Error = Error;
            Order Order = new Order(OrderID);
            ViewBag.OrderStatusOptions = Order.General.GetOrderStatusOptions((int)Enums.OrderStatus.NotSet);
            return View("~/Views/Dashboard/OrderDetails.cshtml", Order);
        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult UpdateOrderStatus(IFormCollection Details)
        {
            Order Order = new Order(Details["OrderID"].ToString());
            Enums.OrderStatus NewOrderStatus = (Enums.OrderStatus)Convert.ToInt32(Details["OrderStatus"].ToString());

            switch (NewOrderStatus)
            {
                case Enums.OrderStatus.Cancelled:

                    //Put the order items back into inventory
                    foreach (OrderProductLink orderProductLink in Order.OrderProductLinks)
                    {
                        Product product = orderProductLink.Product;
                        product.Quantity += orderProductLink.Quantity;
                        product.Save();
                    }

                    Order.OrderStatus = NewOrderStatus;
                    Order.Save();
                    break;
                default:
                    Order.OrderStatus = NewOrderStatus;
                    Order.Save();
                    break;
            }

            return RedirectToAction("ViewDetails", new { @OrderID = Order.Key, @Success = "Order status updated" });
        }
    }
}