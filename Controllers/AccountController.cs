using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Ecommerce.Controllers
{
    public partial class BaseController : Controller
    {
        public async Task<bool> SignUserIn(string ClientID)
        {
            if (!string.IsNullOrEmpty(ClientID))
            {
                Client Client = new Client(ClientID);

                List<Claim> ClientClaims = new List<Claim>();

                ClientClaims.Add(new Claim("ClientID", ClientID));

                List<Enums.ClientRole> ClientRoles = Client.Roles;
                foreach (Enums.ClientRole ClientRole in ClientRoles)
                {
                    ClientClaims.Add(new Claim(ClaimTypes.Role, ClientRole.ToString()));
                }

                ClaimsIdentity ClientIdentity = new ClaimsIdentity(ClientClaims, "Site Identity");

                ClaimsPrincipal ClientPrincipal = new ClaimsPrincipal(new[] { ClientIdentity });

                await HttpContext.SignInAsync(ClientPrincipal);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SignClientOut()
        {
            await HttpContext.SignOutAsync();
            return true;
        }
    }

    public class AccountController : BaseController
    {
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Register(string Error = "")
        {
            if (!string.IsNullOrEmpty(Error))
            {
                ViewBag.Error = Error;
            }

            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public async Task<IActionResult> RegisterUser(IFormCollection Form)
        {
            //Validation
            bool Error = false;
            StringBuilder Message = new StringBuilder();
            //
            if (!Client.Methods.EmailValid(Form["Email"].ToString()))
            {
                Error = true;
                Message.Append("The email entered is invalid");
            }

            if (Client.Methods.EmailExists(Form["Email"].ToString()))
            {
                Error = true;
                Message.Append("The email entered is already registered");
            }

            if (!ValidateRecaptcha(Form["Captcha"].ToString()))
            {
                Error = true;
                Message.Append("Invalid Recaptcha");
            }


            if (!Client.Methods.PasswordValid(Form["Password"].ToString()))
            {
                Error = true;
                Message.Append("your password must be 6 characters in length");
            }

            if (Form["Password"].ToString() != Form["PasswordConfirm"].ToString())
            {
                Error = true;
                Message.Append("Passwords must match");
            }

            if (Error)
            {
                return RedirectToAction("Register", new { Error = Message });
            }
            else
            {
                Client Client = new Client();
                Client.Email = Form["Email"].ToString();
                Client.Password = Form["Password"].ToString();
                if (Client.Save())
                {
                    ClientRole ClientRole = new ClientRole();
                    ClientRole.ClientID = Client.Key;
                    ClientRole.RoleID = Enums.ClientRole.Shopper;
                    if (ClientRole.Save())
                    {
                        await SignUserIn(Client.Key);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Register", new { Error = "Your account could not be created due to an error" });
                    }

                }
                else
                {
                    return RedirectToAction("Register", new { Error = "Your account could not be created due to an error" });
                }

            }
        }

        public IActionResult Login(string Success = "", string Error = "")
        {
            ViewBag.Success = Success;
            ViewBag.Error = Error;
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public async Task<IActionResult> LogClientIn(IFormCollection Details)
        {
            bool Error = false;

            StringBuilder Message = new StringBuilder();
            if (string.IsNullOrEmpty(Details["Email"].ToString()) || string.IsNullOrEmpty(Details["Password"].ToString()))
            {
                Error = true;
                Message.Append("Please enter an Email and Password");
            }

            if (!ValidateRecaptcha(Details["Captcha"].ToString()))
            {
                Error = true;
                Message.Append("Invalid Recaptcha");
            }

            if (!Error)
            {
                Client AuthenticatedClient = Client.Methods.AuthenticateClient(Details["Email"].ToString(), Details["Password"].ToString());

                if (AuthenticatedClient != null)
                {
                    await SignUserIn(AuthenticatedClient.Key);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Message.Append("Incorrect login details");
                }
            }

            return RedirectToAction("LogIn", new { Error = Message });
        }

        public async Task<IActionResult> LogOut()
        {
            await SignClientOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult Manage(string Success = "", string Error = "", Enums.AccountSection Section = Enums.AccountSection.Addresses)
        {
            Client Client = new Client(ClientID);
            ViewBag.Success = Success;
            ViewBag.Error = Error;
            ViewBag.Section = Section;
            return View("~/Views/Account/Manage.cshtml", Client);
        }

        public IActionResult ViewSection(Enums.AccountSection Section)
        {
            return RedirectToAction("Manage", new { @Section = Section });
        }

        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult AddAddress(IFormCollection Details)
        {
            Address Address = new Address();
            Address.PropertyType = (Enums.PropertyType)Convert.ToInt32(Details["PropertyType"].ToString());
            Address.AddressLine = Details["AddressLine"].ToString();
            Address.ZipCode = Convert.ToInt32(Details["ZipCode"].ToString());
            Address.City = Details["City"].ToString();
            Address.Province = (Enums.Province)Convert.ToInt32(Details["Province"].ToString());
            Address.Suburb = Details["Suburb"].ToString();
            Address.ContactNumber = Details["ContactNumber"].ToString();
            Address.DeliveryInstructions = Details["DeliveryInstructions"].ToString();
            Address.ClientID = ClientID;
            if (Address.Save())
            {
                return RedirectToAction("Manage", new { Success = "Address saved successfully" });
            }
            else
            {
                return RedirectToAction("Manage", new { Error = "An error occured while saving the address" });
            }
        }

        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult DeleteAddress(string AddressID)
        {
            Address Address = new Address(AddressID.Decrypt());
            Address.Delete();
            return RedirectToAction("Manage", new { Success = "Address removed successfully" });
        }

        public IActionResult RecoverPassword(string Success = "", string Error = "")
        {
            ViewBag.Success = Success;
            ViewBag.Error = Error;
            return View("~/Views/Account/RecoverPassword.cshtml");
        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult StartPasswordRecovery(IFormCollection Details)
        {
            if (ValidateRecaptcha(Details["Captcha"].ToString()))
            {
                if (Client.Methods.EmailExists(Details["Email"].ToString()))
                {
                    PasswordRecovery Recovery = new PasswordRecovery();
                    Recovery.ClientID = Client.Methods.GetClientIDFromEmail(Details["Email"].ToString());
                    Recovery.Save();
                    ViewBag.RecoveryLink = Startup.StaticConfiguration["SiteInfo:URL"].ToString() + "/Account/ResetPassword?RecoveryID=" + Recovery.Key;
                    if (SendEmail("Password Recovery", RenderViewToStringAsync(this, "~/Views/EmailTemplates/PasswordRecovery.cshtml"), new List<string> { Details["Email"].ToString() }))
                    {
                        return RedirectToAction("RecoverPassword", new { Success = "Please check your email for the recovery link" });
                    }
                    else
                    {
                        return RedirectToAction("RecoverPassword", new { Error = "The recovery email could not be sent" });
                    }
                }
                else
                {
                    return RedirectToAction("RecoverPassword", new { Error = "Email has not been registered" });
                }
            }
            else
            {
                return RedirectToAction("RecoverPassword", new { Error = "Invalid Recaptcha" });
            }
        }

        public IActionResult ResetPassword(string RecoveryID)
        {
            ViewBag.RecoveryID = RecoveryID;
            return View("Views/Account/ResetPassword.cshtml");
        }

        [ValidateAntiForgeryToken, HttpPost]
        public IActionResult SetNewPassword(IFormCollection Details)
        {
            ViewBag.RecoveryID = Details["RecoveryID"].ToString();
            if (Details["Password"].ToString() == Details["ConfirmPassword"])
            {
                PasswordRecovery Recovery = new PasswordRecovery(Details["RecoveryID"].ToString());
                Client Client = new Client(Recovery.ClientID);
                Client.Password = Details["Password"].ToString();
                Client.Save(true);
                return RedirectToAction("Login", new { Success = "Password recovered, please log in" });
            }
            else
            {
                ViewBag.Error = "The passwords entered do not match";
                return View("Views/Account/ResetPassword.cshtml");
            }
        }

        [ValidateAntiForgeryToken, HttpPost]
        public async Task<IActionResult> UpdateDetails(IFormCollection Details)
        {
            StringBuilder ErrorMessage = new StringBuilder();
            if (string.IsNullOrEmpty(Details["Email"].ToString()))
            {
                ErrorMessage.Append("Email is required ");
            }

            if (!ValidateRecaptcha(Details["Captcha"].ToString()))
            {
                ErrorMessage.Append("Invalid Recaptcha ");
            }

            if (Client.Methods.EmailExists(Details["Email"].ToString()))
            {
                ErrorMessage.Append("The email entered is already registered on another account");
            }

            if (!string.IsNullOrEmpty(ErrorMessage.ToString()))
            {
                return RedirectToAction("Manage", new { Error = ErrorMessage.ToString(), @Section = Enums.AccountSection.PersonalDetails });
            }
            else
            {
                Client CurrentClient = new Client(ClientID);
                CurrentClient.Email = Details["Email"].ToString();

                if (CurrentClient.Save())
                {
                    await LogOut();

                    return RedirectToAction("Login", new { Success = "Your email was changed successfully, please log in using your new email address" });
                }
                else
                {
                    return RedirectToAction("Manage", new { Error = "Failed to save new email address", @Section = Enums.AccountSection.PersonalDetails });
                }
            }
        }

        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult ViewOrder(string OrderID)
        {
            Order Order = new Order(OrderID.Decrypt());
            return View("~/Views/Account/OrderDetails.cshtml", Order);
        }

        #region "Wishlists"
        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult CreateWishlist(IFormCollection Details)
        {
            Client Client = new Client(ClientID);
            if (Client.Wishlists.Count > 10)
            {
                return RedirectToAction("Manage", new { Error = "Maximum of 10 wishlists reached", @Section = Enums.AccountSection.Wishlists });
            }
            else
            {
                Wishlist wishlist = new Wishlist();
                wishlist.Name = Details["Name"].ToString();
                wishlist.ClientID = ClientID;
                wishlist.Save();
                return RedirectToAction("Manage", new { Success = "Wishlist Created Successfully", @Section = Enums.AccountSection.Wishlists });
            }
        }

        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult DeleteWishlist(string WishlistID)
        {
            Wishlist Wishlist = new Wishlist(WishlistID.Decrypt());
            foreach (WishlistItem Item in Wishlist.Items)
            {
                Item.Delete();
            }
            Wishlist.Delete();
            return RedirectToAction("Manage", new { Success = "Wishlist Deleted", Section = Enums.AccountSection.Wishlists });
        }

        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult ViewWishlist(string WishlistID, string Success = "", string Error = "")
        {
            Wishlist wishlist = new Wishlist(WishlistID.Decrypt());
            ViewBag.Success = Success;
            ViewBag.Error = Error;
            return View("~/Views/Account/ViewWishlist.cshtml", wishlist);
        }

        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult DeleteWishlistItem(string WishlistItemID)
        {
            WishlistItem Item = new WishlistItem(WishlistItemID.Decrypt());
            Item.Delete();
            return RedirectToAction("ViewWishlist", new { Success = "Wishlist Item Removed Successfully", WishlistID = Item.WishlistID.Encrypt() });
        }

        [Authorize(Roles = "Shopper,Admin")]
        public IActionResult AddToWishlist(string WishlistID, string ProductID)
        {
            WishlistItem Item = new WishlistItem();
            Item.WishlistID = WishlistID.Decrypt();
            Item.ProductID = ProductID.Decrypt();
            if (!Item.Exists())
            {
                Item.Save();
            }
            return RedirectToAction("ViewWishlist", new { WishlistID = WishlistID });
        }
        #endregion
    }
}