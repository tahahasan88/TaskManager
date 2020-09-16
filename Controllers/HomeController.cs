﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskManager.Data;
using TaskManager.Data.Repositories;
using TaskManager.Models;
using TaskManager.Web.Models;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITaskRepository _taskRepository;
        private readonly TaskManagerContext _context;
        public string currentUserName;

        public HomeController(ILogger<HomeController> logger, ITaskRepository taskRepository, TaskManagerContext context, IConfiguration configuration)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _context = context;
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
        }

        public IActionResult Index()
        {
            //string identity = User.Identity.Name;
            //var user = System.Net.CredentialCache.DefaultCredentials;
            //Console.WriteLine(Environment.UserName);

            TaskSummaryViewModel taskSummaryVM = new TaskSummaryViewModel();
            taskSummaryVM.PendingTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
                != (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.Target <= DateTime.Now).Count();

            taskSummaryVM.OverDueTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
                != (int)TaskManager.Common.Common.TaskStatus.Completed
                && DateTime.Now > x.Target).Count();

            taskSummaryVM.ResolvedTodayCount = _context.Tasks.Where(x => x.TaskStatus.Id
                == (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.LastUpdatedAt.Date == DateTime.Now.Date).Count();

            taskSummaryVM.FollowUpsCount = _context.TaskFollowUps.Where(x => x.Task.TaskStatus.Id
                != (int)TaskManager.Common.Common.TaskStatus.Completed).Count();

            ViewData["UserName"] = currentUserName;
            return View(taskSummaryVM);
        }

        public IActionResult List()
        {
            TaskSummaryViewModel taskSummaryVM = new TaskSummaryViewModel();
            taskSummaryVM.PendingTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
                != (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.IsDeleted != true
                && x.Target >= DateTime.Now).Count();

            taskSummaryVM.OverDueTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
                != (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.IsDeleted != true
                && DateTime.Now > x.Target).Count();

            taskSummaryVM.ResolvedTodayCount = _context.Tasks.Where(x => x.TaskStatus.Id
                == (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.IsDeleted != true
                && x.LastUpdatedAt.Date == DateTime.Now.Date).Count();

            taskSummaryVM.FollowUpsCount = _context.TaskFollowUps.Where(x => x.Task.TaskStatus.Id
                != (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.Task.IsDeleted != true
                ).Count();

            return new JsonResult(new { records = taskSummaryVM });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
