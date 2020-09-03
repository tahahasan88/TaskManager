using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Web.Controllers
{
    public class EmployeeDetailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}