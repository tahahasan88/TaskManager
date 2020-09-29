using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TaskManager.Data;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class TaskSummaryController : BaseController
    {
        public string currentUserName;

        public TaskSummaryController(TaskManagerContext context, IConfiguration configuration) : base(context)
        {
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
        }

        private bool IsThisUserManagingThisDepartment(int thisDepartmentId, IEnumerable<Department> departments, string userName)
        {
            bool isThisUserManager = false;
            Department thisDepartment = departments.Where(x => x.Id == thisDepartmentId).SingleOrDefault().ParentDepartment;
            while (thisDepartment != null)
            {
                string managerUserName = _context.Departments.Where(x => x.Id == thisDepartment.Id).Select(x => x.Manager).SingleOrDefault().UserName;
                if (managerUserName == userName)
                {
                    isThisUserManager = true;
                    break;
                }
                thisDepartment = thisDepartment.ParentDepartment;
            }
            return isThisUserManager;
        }

        public IActionResult Index()
        {
            TaskSummaryViewModel taskSummaryVM = new TaskSummaryViewModel();

            var tasks = _context.Tasks
                            .Include(x => x.TaskStatus)
                            .Where(x => x.IsDeleted != true).OrderByDescending(x => x.LastUpdatedAt)
                            .ToList();
            
            var taskEmployees = (from c in _context.TaskEmployees
                                 join o in _context.Employees
                                 on c.UserName equals o.UserName
                                 join tc in _context.TaskCapacities
                                 on c.TaskCapacity.Id equals tc.Id
                                 join dept in _context.Departments
                                 on o.Department.Id equals dept.Id
                                 select new
                                 {
                                     UserName = o.UserName,
                                     EmployeeName = o.EmployeeName,
                                     TaskId = c.Task.Id,
                                     DeptId = o.Department.Id,
                                     ManagerUserName = o.Department.Manager.UserName
                                 }).ToList();

            List<Department> departments = _context.Departments
                    .Include(x => x.ParentDepartment)
                    .Include(x => x.Manager)
                    .ToList();

            foreach (TaskManager.Data.Task task in tasks)
            {
                if (taskEmployees.Where(x => x.TaskId == task.Id && x.UserName == currentUserName).Any()
                    || taskEmployees.Where(x => x.TaskId == task.Id && x.ManagerUserName == currentUserName).Any()
                    || IsThisUserManagingThisDepartment(taskEmployees.Where(x => x.TaskId == task.Id).FirstOrDefault().DeptId,
                        departments, currentUserName))
                {
                    if (task.TaskStatus.Id != (int)TaskManager.Common.Common.TaskStatus.Completed)
                    {
                        taskSummaryVM.PendingTasksCount += 1;
                    }
                    else if (task.TaskStatus.Id == (int)TaskManager.Common.Common.TaskStatus.Completed)
                    {
                        taskSummaryVM.CompletedTasksCount += 1;
                    }
                    else if (task.TaskStatus.Id == (int)TaskManager.Common.Common.TaskStatus.OnHold)
                    {
                        taskSummaryVM.OnHoldTasksCount += 1;
                    }
                    taskSummaryVM.TotalTasksCount += 1;
                }
            }

            return View(taskSummaryVM);
        }

        public IActionResult List()
        {
            try
            {
                TaskSummaryViewModel taskSummaryVM = new TaskSummaryViewModel();
                var tasks = _context.Tasks
                               .Include(x => x.TaskStatus)
                               .Where(x => x.IsDeleted != true).OrderByDescending(x => x.LastUpdatedAt)
                                .Select(x=> new { x.Id, StatusId = x.TaskStatus.Id})
                               .ToList();

                var taskEmployees = (from c in _context.TaskEmployees
                                     join o in _context.Employees
                                     on c.UserName equals o.UserName
                                     join tc in _context.TaskCapacities
                                     on c.TaskCapacity.Id equals tc.Id
                                     join dept in _context.Departments
                                     on o.Department.Id equals dept.Id
                                     select new
                                     {
                                         UserName = o.UserName,
                                         EmployeeName = o.EmployeeName,
                                         TaskId = c.Task.Id,
                                         DeptId = o.Department.Id,
                                         ManagerUserName = o.Department.Manager.UserName
                                     }).ToList();

                IEnumerable<Department> departments = _context.Departments
                        .Include(x => x.ParentDepartment)
                        .Select(s => s);

                foreach (var task in tasks)
                {
                    if (taskEmployees.Where(x => x.TaskId == task.Id && x.UserName == currentUserName).Any()
                        || taskEmployees.Where(x => x.TaskId == task.Id && x.ManagerUserName == currentUserName).Any()
                        || IsThisUserManagingThisDepartment(taskEmployees.Where(x => x.TaskId == task.Id).FirstOrDefault().DeptId,
                            departments, currentUserName))
                    {
                        if (task.StatusId != (int)TaskManager.Common.Common.TaskStatus.Completed)
                        {
                            taskSummaryVM.PendingTasksCount += 1;
                        }
                        else if (task.StatusId == (int)TaskManager.Common.Common.TaskStatus.Completed)
                        {
                            taskSummaryVM.CompletedTasksCount += 1;
                        }
                        else if (task.StatusId == (int)TaskManager.Common.Common.TaskStatus.OnHold)
                        {
                            taskSummaryVM.OnHoldTasksCount += 1;
                        }
                        taskSummaryVM.TotalTasksCount += 1;
                    }
                }

                return new JsonResult(new { records = taskSummaryVM });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}