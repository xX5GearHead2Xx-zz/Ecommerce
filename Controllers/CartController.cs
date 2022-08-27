using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Ecommerce.Models; 

namespace Ecommerce.Controllers
{
    public partial class BaseController : Controller
    {
        public List<Product> GetCartProducts(ListDictionary CartItems)
        {
            List<Product> Products = new List<Product>();
            IDictionaryEnumerator Enumerator = CartItems.GetEnumerator();
            while (Enumerator.MoveNext())
            {
                Product Product = new Product(Enumerator.Key.ToString().Decrypt());
                Product.CartQuantity = Convert.ToInt32(Enumerator.Value);
                Products.Add(Product);
            }
            return Products;
        }

        public ListDictionary GetCartItems()
        {
            ListDictionary CartItems = new ListDictionary();
            if (Request.Cookies["Cart"] != null)
            {
                CartItems = JsonConvert.DeserializeObject<ListDictionary>(Request.Cookies["Cart"].ToString());
            }
            return CartItems;
        }

        CookieOptions Options = new CookieOptions()
        {
            Expires = DateTime.Now.AddHours(1),
        };

        public void SetCartItems(ListDictionary CartItems)
        {
            Response.Cookies.Delete("Cart");
            Response.Cookies.Append("Cart", JsonConvert.SerializeObject(CartItems), Options);
        }

        public void AddCartItem(string ProductID, int Quantity)
        {
            ListDictionary CartItems = GetCartItems();

            if (CartItems[ProductID] != null)
            {
                CartItems.Remove(ProductID);
            }
            CartItems.Add(ProductID, Quantity);

            SetCartItems(CartItems);
        }

        public decimal CalcualteCartTotal(List<Product> CartItems)
        {
            decimal CartTotal = 0;
            foreach (Product CartItem in CartItems)
            {
                CartTotal += (CartItem.Price * CartItem.CartQuantity);
            }
            return CartTotal;
        }

        public void RemoveCartItem(string ProductID)
        {
            ListDictionary CartItems = GetCartItems();
            if (CartItems[ProductID] != null)
            {
                CartItems.Remove(ProductID);
            }
            SetCartItems(CartItems);
        }
    }

    public class CartController : BaseController
    {
    
        string CartMessage;

        public IActionResult ViewCart(string Success = "", string Error = "")
        {
            List<Product> Products = new List<Product>();
            if (Request.Cookies["Cart"] != null)
            {
                ListDictionary CartItems = GetCartItems();
                Products = GetCartProducts(CartItems);
                SetCartItems(CartItems);

                decimal CartTotal = CalcualteCartTotal(Products);

                ViewBag.Success = Success;
                ViewBag.Error = Error;
                ViewBag.CartTotal = CartTotal;
                ViewBag.VAT = CalculateVAT(CartTotal);
            }
            return View("~/Views/Shop/Cart.cshtml", Products);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddToCart(IFormCollection Details, string ProductID)
        {
            int Quantity = Convert.ToInt32(Details["Quantity"].ToString());
            AddCartItem(ProductID, Quantity);
            CartMessage = "Item quantity updated";
            return RedirectToAction("ViewCart", new { Success = CartMessage });
        }

        public IActionResult RemoveFromCart(string ProductID)
        {
            RemoveCartItem(ProductID);
            CartMessage = "Cart Item Removed";
            return RedirectToAction("ViewCart", new { Success = CartMessage });
        }
    }
}