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

        public TaskSummaryController(TaskManagerContext context, IConfiguration configuration) : base(context, configuration)
        {
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
        }


        public IActionResult Index()
        {
            return View();
        }

        protected override bool IsThisUserManagingThisDepartment(Department thisDepartment, string userName)
        {
            bool isThisUserManager = false;

            string managerUserName = _context.Departments.Where(x => x.Id == thisDepartment.Id)
                .Select(x => x.Manager).SingleOrDefault().UserCode;

            if (managerUserName == userName)
            {
                return true;
            }
            Department parentDepartment = thisDepartment.ParentDepartment;
            if (parentDepartment != null)
            {
                thisDepartment = parentDepartment;
                while (thisDepartment != null)
                {
                    managerUserName = _context.Departments.Where(x => x.Id == thisDepartment.Id).Select(x => x.Manager).SingleOrDefault().UserCode;
                    if (managerUserName == userName)
                    {
                        isThisUserManager = true;
                        break;
                    }
                    thisDepartment = thisDepartment.ParentDepartment;
                }
            }
            else
            {
                managerUserName = _context.Departments.Where(x => x.Id == thisDepartment.Id).Select(x => x.Manager).SingleOrDefault().UserCode;
                isThisUserManager = managerUserName == userName;
            }
            return isThisUserManager;
        }

        protected override bool IsThisUserManagingThisDepartment(List<Department> alldepartments, List<int> selectedDepartmentIds, string userName)
        {
            bool isThisUserManager = false;
            try
            {
                List<Department> selectedDepts = alldepartments.Where(x => selectedDepartmentIds.Any(y => y == x.Id)).ToList();

                foreach (Department department in selectedDepts)
                {
                    isThisUserManager = IsThisUserManagingThisDepartment(department, userName);
                    if (isThisUserManager)
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return isThisUserManager;
        }


        public IActionResult List(string username)
        {
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["username"]))
            {
                currentUserName = HttpContext.Request.Query["username"];
            }
            try
            {
                TaskSummaryViewModel taskSummaryVM = new TaskSummaryViewModel();
                var tasks = _context.Tasks
                            .Include(x => x.TaskStatus)
                            .Where(x => x.IsDeleted != true).Select(x => new { x.Id, StatusId = x.TaskStatus.Id }).ToList();

                var taskEmployees = (from c in _context.TaskEmployees
                                     .Where(k => k.IsActive == true && k.Task.IsDeleted == false)
                                     join o in _context.Employees
                                     on c.Employee.UserCode equals o.UserCode
                                     join tc in _context.TaskCapacities
                                     on c.TaskCapacity.Id equals tc.Id
                                     join dept in _context.Departments
                                     on o.Department.Id equals dept.Id
                                     select new
                                     {
                                         UserName = o.UserCode,
                                         EmployeeName = o.EmployeeName,
                                         TaskId = c.Task.Id,
                                         DeptId = o.Department.Id,
                                         ManagerUserName = o.Department.Manager.UserCode
                                     }).ToList();

                List<Department> departments = _context.Departments.Include(x => x.ParentDepartment)
                     .ToList();

                foreach (var task in tasks)
                {
                    if (taskEmployees.Where(x => x.TaskId == task.Id && x.UserName == currentUserName).Any()
                        || taskEmployees.Where(x => x.TaskId == task.Id && x.ManagerUserName == currentUserName).Any()
                        || IsThisUserManagingThisDepartment(
                            departments,
                            taskEmployees.Where(x => x.TaskId == task.Id).Select(x => x.DeptId).Distinct().ToList(),
                           currentUserName))
                    {
                        if (task.StatusId == (int)TaskManager.Common.Common.TaskStatus.Cancelled)
                        {
                            taskSummaryVM.CancelledTasksCount += 1;
                        }
                        else if (task.StatusId == (int)TaskManager.Common.Common.TaskStatus.Completed)
                        {
                            taskSummaryVM.CompletedTasksCount += 1;
                        }
                        else if (task.StatusId == (int)TaskManager.Common.Common.TaskStatus.InProgress)
                        {
                            taskSummaryVM.InProgressTasksCount += 1;
                        }
                        else if (task.StatusId == (int)TaskManager.Common.Common.TaskStatus.NotStarted)
                        {
                            taskSummaryVM.NotStartedTasksCount += 1;
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
                Console.WriteLine(ex.StackTrace);
                throw ex;
            }
        }
    }
}