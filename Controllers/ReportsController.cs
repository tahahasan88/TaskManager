using System;
using Dapper;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TaskManager.Data;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly IConfiguration _iconfiguration;
        public string currentUserName;

        public ReportsController(IConfiguration configuration, TaskManagerContext context)
        {
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
            _iconfiguration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["UserName"] = currentUserName;
            return View();
        }

        public bool IsThisUserManagingThisDepartment(Department thisDepartment, string userName)
        {
            bool isThisUserManager = false;
            thisDepartment = thisDepartment.ParentDepartment;
            while (thisDepartment != null)
            {
                if (thisDepartment.Manager.UserName == userName)
                {
                    isThisUserManager = true;
                    break;
                }
                thisDepartment = thisDepartment.ParentDepartment;
            }
            return isThisUserManager;
        }

        public IActionResult LoadReportsData()
        {
            var connectionString = _iconfiguration.GetConnectionString("DefaultConnection");
            Dictionary<string, List<TaskData>> userTaskDictionary = new Dictionary<string, List<TaskData>>();
            int tagCounter = 0;

            GetTaskDatabyTaskStatus(connectionString, userTaskDictionary);
            List<Employee> employees = _context.Employees.Include(x => x.Department).ToList();
            List<Department> departments = _context.Departments.Include(x => x.Manager).OrderBy(x => x.Id).ToList();
            Employee loggedInEmployee = employees
                .Where(x => x.UserName == currentUserName).FirstOrDefault();
            bool isHeadOfDepartments = loggedInEmployee.Department.ParentDepartment == null ? true : false;

            List<ReportsViewModel> reportsVM = new List<ReportsViewModel>();
            //ReportsViewModel firstRow = new ReportsViewModel();
            //firstRow.TagId = ++tagCounter;
            //firstRow.IsDeptName = true;
            //firstRow.TagName = departments.FirstOrDefault().Name;
            //reportsVM.Add(firstRow);

            List<Department> restOfDepartments = departments.Where(x => x.ParentDepartment != null).ToList();
            foreach (Department department in departments)
            {
                ReportsViewModel reportRow = new ReportsViewModel();
                reportRow.TagName = department.Name;
                reportRow.TagId = ++tagCounter;
                reportRow.IsDeptName = true;
                reportsVM.Add(reportRow);

                foreach (Employee employee in employees.Where(x => x.Department.Id == department.Id))
                {
                    if (userTaskDictionary.ContainsKey(employee.UserName))
                    {
                        if (isHeadOfDepartments || IsThisUserManagingThisDepartment(department, currentUserName) || employee.Department == loggedInEmployee.Department)
                        {
                            CreateReportRows(userTaskDictionary, reportsVM, employee, ++tagCounter);
                        }
                    }
                }
            }
            int recordsTotal = reportsVM.Count;

            return Json(new { draw = "1", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = reportsVM });
        }

        private void CreateReportRows(Dictionary<string, List<TaskData>> userTaskDictionary, List<ReportsViewModel> reportsVM, Employee employee, int tagId)
        {
            ReportsViewModel newRow = new ReportsViewModel();
            newRow.TagName = employee.EmployeeName;
            newRow.TagId = tagId;
            newRow.TagCount = userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.AllTasks)
                .SingleOrDefault().TaskCount;
            if (userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.Cancelled).Count() > 0)
            {
                newRow.CancelledTasksCount = userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.Cancelled).SingleOrDefault().TaskCount;
            }
            if (userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.Completed).Count() > 0)
            {
                newRow.CompletedTasksCount = userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.Completed).SingleOrDefault().TaskCount;
            }
            if (userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.InProgress).Count() > 0)
            {
                newRow.InProgressTasksCount = userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.InProgress).SingleOrDefault().TaskCount;
            }
            if (userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.NotStarted).Count() > 0)
            {
                newRow.NotStartedTasksCount = userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.NotStarted).SingleOrDefault().TaskCount;
            }
            if (userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.OnHold).Count() > 0)
            {
                newRow.OnHoldTasksCount = userTaskDictionary[employee.UserName].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.OnHold).SingleOrDefault().TaskCount;
            }
            newRow.TagUserName = employee.UserName;
            reportsVM.Add(newRow);
        }

        private void GetTaskDatabyTaskStatus(string connectionString, Dictionary<string, List<TaskData>> userTaskDictionary)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sqlQuery = "select taskdata.UserName, taskdata.TaskStatusId, count(taskdata.id) as taskCount from " +
                    "(select distinct Tasks.id, Tasks.TaskStatusId, TaskEmployees.UserName from Tasks " +
                    "inner join TaskEmployees on TaskEmployees.TaskId = Tasks.Id " +
                    "where TaskEmployees.IsActive = 1 " +
                    "and Tasks.isdeleted = 0) " +
                    "as taskdata " +
                    "group by taskdata.TaskStatusId,taskdata.UserName";

                conn.Open();
                var queryResult = conn.Query(sqlQuery);
                foreach (var result in queryResult)
                {
                    if (userTaskDictionary.ContainsKey(result.UserName))
                    {
                        userTaskDictionary[result.UserName].Add(new TaskData()
                        {
                            TaskCount = result.taskCount,
                            TaskStatusId = result.TaskStatusId
                        });
                    }
                    else
                    {
                        userTaskDictionary.Add(result.UserName, new List<TaskData>() { new TaskData()
                        {   TaskStatusId = result.TaskStatusId,
                            TaskCount = result.taskCount
                        } });
                    }
                    List<TaskData> existingUserTaskList = userTaskDictionary[result.UserName];
                    if (existingUserTaskList.Any(x => x.TaskStatusId == (int)Common.Common.TaskStatus.AllTasks))
                    {
                        TaskData thisTaskData = existingUserTaskList.Where(x => x.TaskStatusId == 0).SingleOrDefault();
                        thisTaskData.TaskCount += result.taskCount;
                    }
                    else
                    {
                        userTaskDictionary[result.UserName].Add(new TaskData()
                        {
                            TaskStatusId = (int)Common.Common.TaskStatus.AllTasks,
                            TaskCount = result.taskCount
                        });
                    }
                }
            }
        }
    }
}