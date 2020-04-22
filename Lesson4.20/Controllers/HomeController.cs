using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Lesson4._20.Models;
using Lesson4._20.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Lesson4._20.Controllers
{
    public class HomeController : Controller
    {
        SimpleAdSiteDb db = new SimpleAdSiteDb("Data Source=.\\sqlexpress;Initial Catalog=SimpleAdsSite;Integrated Security=True");

        public IActionResult Index()
        {
           
            var vm=new IndexViewModel
            {
                IsLoggedIn = User.Identity.IsAuthenticated,
                Ads = db.GetAds(),
                
        };
            if (vm.IsLoggedIn)
            {              
                var user = db.GetByEmail(User.Identity.Name);
                vm.UserId = user.Id;
            }
            return View(vm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            var user = db.GetByEmail(User.Identity.Name);
            
            return View(user.Id);
        }
        
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            db.NewAd(ad);
            return Redirect("/");
        }
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            List<int> ids = HttpContext.Session.Get<List<int>>("ListingIds");
            if (ids != null && ids.Contains(id))
            {
                db.Delete(id);
            }

            return Redirect("/");
        }
       
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
        
    }
}
