using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class EmployeeDetailController : Controller
    {
        public IActionResult Index()
        {
            TaskCreateViewModel createVM = new TaskCreateViewModel();
            return PartialView("Index", createVM);
        }

        public IActionResult Details()
        {
            TaskCreateViewModel createVM = new TaskCreateViewModel();
            return View(createVM);
        }
    }
}