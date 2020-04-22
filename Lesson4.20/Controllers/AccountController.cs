using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Lesson4._20.Data;
using Lesson4._20.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Lesson4._20.Controllers
{
    public class AccountController : Controller
    {
        SimpleAdSiteDb db = new SimpleAdSiteDb("Data Source=.\\sqlexpress;Initial Catalog=SimpleAdsSite;Integrated Security=True");
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            db.AddUser(user, password);
            return Redirect("/account/signup");
        }
        public IActionResult Login()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Message = TempData["Error"];
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = db.Login(email, password);
            if (user == null)
            {
                TempData["Error"] = "Invalid Email or Password!";
                return Redirect("/account/login");
            }
            var claims = new List<Claim>
            {
                new Claim("user", email)
            };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/newad");
        }
        public  IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
        public IActionResult MyAccount()
        {
           var user = db.GetByEmail(User.Identity.Name);
            return View(new MyAccountViewModel
            {
                Ads = db.GetById(user.Id)
            });
           
        }
    }
}