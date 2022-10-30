using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace Ecommerce.Controllers
{
    public class PaymentController : BaseController
    {
        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult ProceedToCheckout(string Error = "")
        {
            ViewBag.Error = Error;
            List<Address> Addresses = Address.Methods.GetClientAddresses(ClientID);
            return View("~/Views/Checkout/DeliveryMethod.cshtml", Addresses);
        }

        #region "Paygate"
        [ValidateAntiForgeryToken, HttpPost, Authorize(Roles = "Shopper,Admin")]
        public IActionResult ProceedToPayment(IFormCollection Details)
        {
            Enums.PaymentOption PaymentOption = (Enums.PaymentOption)Convert.ToInt32(Ecommerce.Startup.StaticConfiguration["PaymentSettings:PaymentOption"]);

            if (HttpContext.Request.Cookies["Cart"] != null && !string.IsNullOrEmpty(Details["DeliveryAddress"].ToString()))
            {
                ListDictionary CartItems = JsonConvert.DeserializeObject<ListDictionary>(HttpContext.Request.Cookies["Cart"].ToString());
                List<Product> Products = GetCartProducts(CartItems);

                decimal OrderTotal = CalcualteCartTotal(Products);
                if (Products.Count() > 0)
                {
                    Client Client = new Client(ClientID);

                    Order Order = new Order();
                    Order.ClientID = ClientID;
                    Order.DeliveryAddressID = Details["DeliveryAddress"].ToString();
                    Order.OrderStatus = Enums.OrderStatus.Unfinalised;
                    Order.PaymentOption = PaymentOption;
                    Order.Save();
                    Order = new Order(Order.Key);//load the order so we get the number

                    //Link Product to order
                    List<Product> RemovedProducts = new List<Product>();
                    foreach (Product Product in Products)
                    {
                        if (Product.Quantity >= Product.CartQuantity)
                        {
                            OrderProductLink OrderProductLink = new OrderProductLink()
                            {
                                ProductID = Product.Key,
                                OrderID = Order.Key,
                                Quantity = Product.CartQuantity,
                                ItemPrice = Product.Price
                            };
                            OrderProductLink.Save();
                        }
                        else
                        {
                            RemovedProducts.Add(Product);
                            RemoveCartItem(Product.Key);
                        }
                    }

                    if (RemovedProducts.Count > 0)
                    {
                        ViewBag.Error = "Some items have been removed from your order as they are no longer available";
                    }

                    TransactionLog TransactionLog;
                    switch (PaymentOption)
                    {
                        case Enums.PaymentOption.Paygate:
                            Dictionary<string, string> Response = new PaygateRequest(Convert.ToInt32(OrderTotal), Order.Number.ToString(), Client.Email).InitiateTransaction();
                            TransactionLog = new TransactionLog();
                            TransactionLog.PayRequestID = Response.Where(R => R.Key == "PAY_REQUEST_ID").First().Value;
                            TransactionLog.Checksum = Response.Where(R => R.Key == "CHECKSUM").First().Value;
                            TransactionLog.OrderID = Order.Key;
                            TransactionLog.Amount = Convert.ToInt32(OrderTotal * 100);
                            TransactionLog.ClientID = ClientID;
                            TransactionLog.Save();
                            ViewBag.RedirectURL = Startup.StaticConfiguration["PaygateSettings:RedirectURL"].ToString() + "?" + TransactionLog.PayRequestID;
                            ViewBag.RedirectParams = Response;
                            break;
                        case Enums.PaymentOption.Manual:
                            TransactionLog = new TransactionLog();
                            TransactionLog.OrderID = Order.Key;
                            TransactionLog.Amount = Convert.ToInt32(OrderTotal * 100);
                            TransactionLog.ClientID = ClientID;
                            TransactionLog.Save();
                            ViewBag.Success = "Order #" + Order.Number + " Received";
                            ViewBag.BankDetails = new BankAccountDetails();
                            return View("~/Views/Checkout/Confirmation.cshtml", Order);
                    }

                    ViewBag.OrderTotal = OrderTotal;
                    return View("~/Views/Checkout/OrderDetails.cshtml", Order);
                }
            }
            return RedirectToAction("ViewCart", "Cart", new { Error = "Checkout failed, missing cart or delivery address" });
        }

        [AllowAnonymous, HttpPost]
        public IActionResult ReturnClient(string PAY_REQUEST_ID)
        {
            //make sure this request comes from paygate
            if (Request.Headers["Origin"].ToString().Contains("paygate"))
            {
                TransactionLog TransactionLog = Models.TransactionLog.Methods.GetTransactionLogByPayRequestID(PAY_REQUEST_ID);
                Order Order = new Order(TransactionLog.OrderID);
                Task.Run(() => SignUserIn(TransactionLog.ClientID));

                PaygateQuery query = new PaygateQuery();
                query.Reference = Order.Number.ToString();
                query.PayRequestID = TransactionLog.PayRequestID;
                query.Checksum = TransactionLog.Checksum;
                Dictionary<string, string> QueryResponse = query.QueryTransaction();

                TransactionLog.StatusCode = QueryResponse.Where(R => R.Key == "TRANSACTION_STATUS").First().Value;
                TransactionLog.Description = QueryResponse.Where(R => R.Key == "RESULT_DESC").First().Value;
                TransactionLog.Save();

                bool Success = false;

                //Update Order Status if payment received
                if (TransactionLog.StatusCode == "1")
                {
                    Order.OrderStatus = Enums.OrderStatus.Payed;
                    Order.Save();
                    Success = true;
                }

                if (Success)
                {
                    //Update Product Quantities
                    List<OrderProductLink> OrderProductLinks = OrderProductLink.Methods.GetProductsLinkedToOrder(Order.Key);
                    foreach (OrderProductLink OrderLink in OrderProductLinks)
                    {
                        Product Product = new Product(OrderLink.ProductID);
                        Product.Quantity = Product.Quantity - OrderLink.Quantity;
                        Product.Save();
                    }

                    Client Client = new Client(TransactionLog.ClientID);
                    ViewBag.OrderDetails = Order;
                    ViewBag.Success = "Order #" + Order.Number + " Received";

                    //Email Invoice
                    ViewBag.Order = Order;

                    SendEmail("Order Receipt #" + Order.Number, RenderViewToStringAsync(this, "~/Views/EmailTemplates/Invoice.cshtml"), new List<string> { Client.Email });

                    //Clear the shopping cart
                    Response.Cookies.Delete("Cart");

                    ViewBag.Success = "Order #" + Order.Number + " Received";
                    return View("~/Views/Checkout/Confirmation.cshtml", Order);
                }
                else
                {
                    StringBuilder ErrorMessage = new StringBuilder();
                    ErrorMessage.Append("Your transaction could not be processed: ");
                    ErrorMessage.Append(TransactionLog.Description);

                    return RedirectToAction("ProceedToCheckout", new { Error = ErrorMessage });
                }
            }
            else
            {
                return View("~/Views/Account/AccessDenied.cshtml");
            }
        }
        #endregion

        [AllowAnonymous]
        public IActionResult GoToOrder(string OrderID)
        {
            Order Order = new Order(OrderID.Decrypt());
            return View("~/Views/Account/OrderDetails.cshtml", Order);
        }
    }
}
