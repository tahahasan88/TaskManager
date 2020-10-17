using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskManager.Data;
using TaskManager.Data.Repositories;
using TaskManager.Models;
using TaskManager.Web.Controllers;
using TaskManager.Web.Models;

namespace TaskManager.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IConfiguration configuration, TaskManagerContext context) : base(context, configuration)
        {
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
        }

        public IActionResult Index()
        {
            SetUserName();
            //TaskSummaryViewModel taskSummaryVM = new TaskSummaryViewModel();
            //taskSummaryVM.PendingTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
            //    != (int)TaskManager.Common.Common.TaskStatus.Completed
            //    && x.Target <= DateTime.Now).Count();

            //taskSummaryVM.OverDueTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
            //    != (int)TaskManager.Common.Common.TaskStatus.Completed
            //    && DateTime.Now > x.Target).Count();

            //taskSummaryVM.ResolvedTodayCount = _context.Tasks.Where(x => x.TaskStatus.Id
            //    == (int)TaskManager.Common.Common.TaskStatus.Completed
            //    && x.LastUpdatedAt.Date == DateTime.Now.Date).Count();

            //taskSummaryVM.FollowUpsCount = _context.TaskFollowUps.Where(x => x.Task.TaskStatus.Id
            //    != (int)TaskManager.Common.Common.TaskStatus.Completed).Count();

            ViewData["UserName"] = currentUserName;
            Employee employee = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
            ViewData["EmployeeName"] = employee == null ? "" : employee.EmployeeName;
            return View();
        }

        private int GetTaskFollowUpsCount(string userName)
        {
            string connectionString = _iconfiguration.GetConnectionString("DefaultConnection");
            string sqlQuery = string.Empty;
            int followUpCount = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                sqlQuery = "select count(distinct(TaskFollowUps.Id)) as followupCount from TaskFollowUps" +
                   " inner join TaskEmployees on TaskEmployees.TaskId = TaskFollowUps.TaskId" +
                   " inner join Employees on Employees.Id = TaskEmployees.EmployeeId" + 
                   " inner join Tasks on Tasks.Id = TaskEmployees.TaskId" +
                   " and TaskFollowUps.StatusId = " + ((int)Common.Common.TaskFollowUpStatus.Open).ToString() +
                   " and Tasks.IsDeleted = 0" +
                   " and TaskEmployees.IsActive = 1" +
                   " and Employees.UserCode = '" + userName + "'" +
                   " and TaskEmployees.TaskCapacityId = " + ((int)Common.Common.TaskCapacity.Assignee).ToString();
                conn.Open();

                var queryResult = conn.Query(sqlQuery);
                foreach (var result in queryResult)
                {
                    if (result.followupCount == null)
                    {
                        followUpCount = 0;
                    }
                    else
                        followUpCount = result.followupCount;
                }
            }
            return followUpCount;
        }

        public IActionResult List(string username)
        {
            TaskSummaryViewModel taskSummaryVM = new TaskSummaryViewModel();

            if (!String.IsNullOrEmpty(username))
            {
                currentUserName = username;
            }
            var tasks = _context.Tasks
                        .Where(x => x.IsDeleted != true).Select(x => new { x.Id, StatusId = x.TaskStatus.Id,
                            IsDeleted = x.IsDeleted,
                            Target = x.Target,
                            LastUpdatedAt = x.LastUpdatedAt
                        }).ToList();

            taskSummaryVM.FollowUpsCount = GetTaskFollowUpsCount(username);

            var taskEmployees = (from c in _context.TaskEmployees
                                 .Where(k => k.IsActive == true && k.Task.IsDeleted == false
                                 && k.TaskCapacity.Id == (int)(Common.Common.TaskCapacity.Assignee))
                                 join o in _context.Employees
                                 on c.Employee.UserCode equals o.UserCode
                                 join tc in _context.TaskCapacities
                                 on c.TaskCapacity.Id equals tc.Id
                                 join dept in _context.Departments
                                 on o.Department.Id equals dept.Id
                                 select new
                                 {
                                     UserName = o.UserCode,
                                     TaskId = c.Task.Id,
                                     DeptId = o.Department.Id,
                                     ManagerUserName = o.Department.Manager.UserCode
                                 }).ToList();

            List<Department> departments = _context.Departments
                .Include(x => x.ParentDepartment)
                .Include(x => x.Manager).OrderBy(x => x.Id)
                .ToList();

            foreach (var task in tasks)
            {
                if (taskEmployees.Where(x => x.TaskId == task.Id && x.UserName == currentUserName).Any())
                {
                    if (task.StatusId != (int)TaskManager.Common.Common.TaskStatus.Completed
                        && task.IsDeleted != true && task.Target.Date >= DateTime.Now.Date)
                    {
                        taskSummaryVM.PendingTasksCount += 1;
                    }
                    if (task.StatusId != (int)TaskManager.Common.Common.TaskStatus.Completed
                       && task.IsDeleted != true && DateTime.Now.Date > task.Target.Date)
                    {
                        taskSummaryVM.OverDueTasksCount += 1;
                    }
                    if (task.StatusId == (int)TaskManager.Common.Common.TaskStatus.Completed
                       && task.IsDeleted != true && task.LastUpdatedAt.Date == DateTime.Now.Date)
                    {
                        taskSummaryVM.ResolvedTodayCount += 1;
                    }
                }
            }


            return new JsonResult(new { records = taskSummaryVM });
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
