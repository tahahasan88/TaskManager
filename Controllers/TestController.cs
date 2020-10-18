using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            ViewData["IdentityUser"] = Environment.UserName;
            return View();
        }
    }
}