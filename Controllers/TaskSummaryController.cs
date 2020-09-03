using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class TaskSummaryController : Controller
    {
        private readonly TaskManagerContext _context;
        public TaskSummaryController(TaskManagerContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            TaskSummaryViewModel taskSummaryVM = new TaskSummaryViewModel();
            taskSummaryVM.PendingTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id 
                != (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.Target <= DateTime.Now).Count();

            taskSummaryVM.CompletedTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
               == (int)TaskManager.Common.Common.TaskStatus.Completed).Count();

            taskSummaryVM.OnHoldTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
               == (int)TaskManager.Common.Common.TaskStatus.OnHold).Count();

            taskSummaryVM.TotalTasksCount = _context.Tasks.Count();

            return View(taskSummaryVM);
        }
    }
}