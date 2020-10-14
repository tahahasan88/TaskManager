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
    public class ReportsController : BaseController
    {
        public ReportsController(IConfiguration configuration, TaskManagerContext context) : base(context, configuration)
        {
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
        }

        public IActionResult Index()
        {
            SetUserName();
            ViewData["UserName"] = currentUserName;
            Employee employee = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
            ViewData["EmployeeName"] = employee == null ? "" : employee.EmployeeName;
            return View();
        }

        public IActionResult LoadReportsData(string username)
        {
            if (!String.IsNullOrEmpty(username))
            {
                currentUserName = username;
            }
            var connectionString = _iconfiguration.GetConnectionString("DefaultConnection");
            Dictionary<string, List<TaskData>> userTaskDictionary = new Dictionary<string, List<TaskData>>();
            int tagCounter = 0;

            GetUserData(connectionString, userTaskDictionary);
            List<Employee> employees = _context.Employees.Include(x => x.Department).ToList();
            List<Department> departments = _context.Departments
                .Include(x => x.ParentDepartment)
                .Include(x => x.Manager).OrderBy(x => x.Id).ToList();

            List<ReportsViewModel> reportsVM = new List<ReportsViewModel>();

            foreach (Department department in departments)
            {
                ReportsViewModel reportRow = new ReportsViewModel();
                reportRow.TagName = department.Name;
                reportRow.TagId = ++tagCounter;
                reportRow.IsDeptName = true;
                int gridViewUiDepthLevel = GetDeptLevel(department);
                reportRow.DepartmentLevel = gridViewUiDepthLevel;

                reportsVM.Add(reportRow);
                gridViewUiDepthLevel += 4;
                bool baseDept = false;
                bool isThisEmployeeManager = IsThisUserManagingThisDepartment(department, currentUserName);

                foreach (Employee employee in employees.Where(x => x.Department != null && x.Department.Id == department.Id))
                {
                    if (userTaskDictionary.ContainsKey(employee.UserCode))
                    {
                        if (isThisEmployeeManager
                            || employee.UserCode == currentUserName
                            || department.Manager.UserCode == currentUserName
                            )
                        {
                            if (!baseDept)
                            {
                                baseDept = true;
                                reportsVM[reportsVM.Count - 1].IsBaseDepartment = true;
                            }
                            CreateReportRows(userTaskDictionary, reportsVM, employee, ++tagCounter, gridViewUiDepthLevel);
                        }
                    }
                    else if (isThisEmployeeManager || employee.UserCode == currentUserName)
                    {
                        CreateReportRows(userTaskDictionary, reportsVM, employee, ++tagCounter, gridViewUiDepthLevel);
                    }
                }
            }
            int recordsTotal = reportsVM.Count;

            return Json(new { draw = "1", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = reportsVM });
        }

        private void CreateReportRows(Dictionary<string, List<TaskData>> userTaskDictionary,
            List<ReportsViewModel> reportsVM, Employee employee, int tagId, int deptLevel)
        {
            ReportsViewModel newRow = new ReportsViewModel();
            newRow.TagName = employee.EmployeeName;
            newRow.TagId = tagId;
            newRow.DepartmentLevel = deptLevel;
            if (userTaskDictionary.ContainsKey(employee.UserCode))
            {
                newRow.TagCount = userTaskDictionary[employee.UserCode].
                Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.AllTasks)
                .SingleOrDefault().TaskCount;

                if (userTaskDictionary[employee.UserCode].
               Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.Cancelled).Count() > 0)
                {
                    newRow.CancelledTasksCount = userTaskDictionary[employee.UserCode].
                    Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.Cancelled).SingleOrDefault().TaskCount;
                }
                if (userTaskDictionary[employee.UserCode].
                    Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.Completed).Count() > 0)
                {
                    newRow.CompletedTasksCount = userTaskDictionary[employee.UserCode].
                    Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.Completed).SingleOrDefault().TaskCount;
                }
                if (userTaskDictionary[employee.UserCode].
                    Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.InProgress).Count() > 0)
                {
                    newRow.InProgressTasksCount = userTaskDictionary[employee.UserCode].
                    Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.InProgress).SingleOrDefault().TaskCount;
                }
                if (userTaskDictionary[employee.UserCode].
                    Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.NotStarted).Count() > 0)
                {
                    newRow.NotStartedTasksCount = userTaskDictionary[employee.UserCode].
                    Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.NotStarted).SingleOrDefault().TaskCount;
                }
                if (userTaskDictionary[employee.UserCode].
                    Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.OnHold).Count() > 0)
                {
                    newRow.OnHoldTasksCount = userTaskDictionary[employee.UserCode].
                    Where(x => x.TaskStatusId == (int)Common.Common.TaskStatus.OnHold).SingleOrDefault().TaskCount;
                }
            }
            else
            {
                newRow.TagCount = 0;
                newRow.CancelledTasksCount = 0;
                newRow.CompletedTasksCount = 0;
                newRow.InProgressTasksCount = 0;
                newRow.NotStartedTasksCount = 0;
                newRow.OnHoldTasksCount = 0;
            }

            newRow.AvatarImage = employee.AvatarImage;
            newRow.TagUserName = employee.UserCode;
            reportsVM.Add(newRow);
        }


        public IActionResult GetReportDetail()
        {
            return PartialView("_ReportUserDetail");
        }

        public IActionResult GetTaskList(string userName, int statusId)
        {
            try
            {
                List<TasksGridViewModel> tasksGridVMList = new List<TasksGridViewModel>();
                var connectionString = _iconfiguration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlQuery = "select distinct tasks.Id, tasks.Title, tasks.TaskStatusId  from tasks" +
                                    " inner join taskEmployees on taskEmployees.TaskId = tasks.Id" +
                                    " inner join Employees on Employees.Id = taskEmployees.EmployeeId" +
                                    " where taskEmployees.IsActive = 1" +
                                    " and Employees.UserCode = '" + userName + "'" +
                                    " and tasks.IsDeleted = 0";

                    sqlQuery = sqlQuery + ((statusId == 0) ? "" : (" and TaskStatusId = " + statusId + ""));

                    conn.Open();
                    var tasks = conn.Query(sqlQuery);
                    foreach (var task in tasks)
                    {
                        tasksGridVMList.Add(new TasksGridViewModel()
                        {
                            TaskId = task.Id,
                            Title = task.Title,
                            Status = ((Common.Common.TaskStatus)(task.TaskStatusId)).ToString()
                        });
                    }
                    int recordsTotal = tasksGridVMList.Count;

                    return Json(new { draw = "1", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = tasksGridVMList });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}