using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TaskManager.Data;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class BaseController : Controller
    {
        public readonly TaskManagerContext _context;
        protected string currentUserName;
        protected IConfiguration _iconfiguration;

        public BaseController(TaskManagerContext context, IConfiguration config)
        {
            _context = context;
            _iconfiguration = config;
        }

        protected void SetUserName()
        {
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["username"]))
            {
                currentUserName = HttpContext.Request.Query["username"];
            }
        }

        public List<string> GetTaskEmployeeEmailAddresses(int taskId, string currentUserName)
        {
            List<TaskEmployee> taskEmployees =
                _context.TaskEmployees.Where(x => x.Task.Id == taskId).ToList();
            List<string> listOfEmployeeEmailAddress = new List<string>();

            //EmployeeList employeeList = new EmployeeList();
            List<Employee> employees = _context.Employees.Include(x => x.Department).ToList();
            foreach (TaskEmployee employee in taskEmployees)
            {
                if (employee.UserName != null && employee.UserName != currentUserName)
                {
                    listOfEmployeeEmailAddress.Add(employees.Where(x => x.UserName == employee.UserName)
                        .SingleOrDefault().EmailAddress);
                }
            }
            return listOfEmployeeEmailAddress;
        }

        protected bool IsThisUserManagingThisDepartment(Department thisDepartment, string userName)
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

        protected int GetDeptLevel(Department thisDept)
        {
            int depthLevel = 0;
            while (thisDept.ParentDepartment != null)
            {
                thisDept = thisDept.ParentDepartment;
                depthLevel = depthLevel + 2;
            }
            return depthLevel;
        }

        protected void GetTaskDatabyTaskStatus(string connectionString, Dictionary<string, List<TaskData>> userTaskDictionary)
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

        protected List<AssigneeDropDownViewModel> LoadAssigneeDrpDwnData(string username)
        {
            var connectionString = _iconfiguration.GetConnectionString("DefaultConnection");
            Dictionary<string, List<TaskData>> userTaskDictionary = new Dictionary<string, List<TaskData>>();
            int tagCounter = 0;

            List<Employee> employees = _context.Employees.Include(x => x.Department).ToList();
            List<Department> departments = _context.Departments
                .Include(x => x.ParentDepartment)
                .Include(x => x.Manager).OrderBy(x => x.Id).ToList();
            GetTaskDatabyTaskStatus(connectionString, userTaskDictionary);

            List<AssigneeDropDownViewModel> assigneeVM = new List<AssigneeDropDownViewModel>();

            foreach (Department department in departments)
            {
                int gridViewUiDepthLevel = 0;
                AssigneeDropDownViewModel assigneeRow = new AssigneeDropDownViewModel();
                assigneeRow.TagName = department.Name;
                assigneeRow.TagId = ++tagCounter;
                assigneeRow.IsDeptName = true;
                gridViewUiDepthLevel = GetDeptLevel(department);
                assigneeRow.DepartmentLevel = gridViewUiDepthLevel;

                assigneeVM.Add(assigneeRow);
                gridViewUiDepthLevel += 2;

                foreach (Employee employee in employees.Where(x => x.Department.Id == department.Id))
                {
                    if (userTaskDictionary.ContainsKey(employee.UserName))
                    {
                        if (IsThisUserManagingThisDepartment(department, username)
                            || employee.UserName == username
                            || department.Manager.UserName == username
                            )
                        {
                            CreateAssigneeRows(userTaskDictionary, assigneeVM, employee, ++tagCounter, gridViewUiDepthLevel);
                        }
                    }
                }
            }
            int recordsTotal = assigneeVM.Count;

            return assigneeVM;
        }

        private void CreateAssigneeRows(Dictionary<string, List<TaskData>> userTaskDictionary,
            List<AssigneeDropDownViewModel> assigneeVM, Employee employee, int tagId, int deptLevel)
        {
            AssigneeDropDownViewModel newRow = new AssigneeDropDownViewModel();
            newRow.TagName = employee.EmployeeName;
            newRow.TagId = tagId;
            newRow.DepartmentLevel = deptLevel;
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
            assigneeVM.Add(newRow);
        }

    }
}