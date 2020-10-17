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
    public class EmployeeDetailController : Controller
    {
        public string currentUserName = "";
        private readonly TaskManagerContext _context;
        private readonly IConfiguration _iconfiguration;


        public EmployeeDetailController(TaskManagerContext context, IConfiguration configuration)
        {
            _context = context;
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
            _iconfiguration = configuration;
        }

        private static int GetSortOrderOfStatus(int statusId)
        {
            
            if ((Common.Common.TaskFollowUpStatus)(statusId) == Common.Common.TaskFollowUpStatus.Open){
                return 1;
            }
            return 2;
        }

        private static int GetSortOrderOfStatus(string statusName)
        {

            if (statusName == "Open")
            {
                return 1;
            }
            return 2;
        }

        public IActionResult LoadEmployeeTasksData(string userName)
        {
            try
            {
                userName = userName == null ? currentUserName : userName;
                EmployeeViewModel employeeVM = new EmployeeViewModel();
                employeeVM.TasksGridVMList = new List<TasksGridViewModel>();

                var connectionString = _iconfiguration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlQuery = "select distinct Tasks.Id," +
                    "Employees.UserCode," +
                    "TaskStatus.Status," +
                    "Tasks.TaskProgress," +
                    "Tasks.Title," +
                    "Tasks.LastUpdatedAt," +
                    "Tasks.Target," +
                    "TaskStatus.Id as TaskStatusId" +
                    " from TaskEmployees" +
                    " inner join Employees on Employees.Id = TaskEmployees.EmployeeId" + 
                    " inner join Tasks on Tasks.Id = TaskEmployees.TaskId" +
                    " inner join TaskStatus on TaskStatus.Id = Tasks.TaskStatusId" +
                    " where Employees.UserCode = '" + userName + "'" +
                    " and TaskEmployees.IsActive = 1" +
                    " and Tasks.IsDeleted = 0" +
                    " order by LastUpdatedAt desc";

                    conn.Open();
                    var taskEmployees = conn.Query(sqlQuery);
                    foreach (var taskEmployee in taskEmployees)
                    {
                        employeeVM.TasksGridVMList.Add(new TasksGridViewModel()
                        {
                            Title = taskEmployee.Title,
                            Status = taskEmployee.Status,
                            TargetDate = taskEmployee.Target.ToString("dd-MMM-yyyy HH:mm:ss"),
                            TaskId = taskEmployee.Id,
                            SortId = GetSortOrderOfStatus(taskEmployee.TaskStatusId)
                        });
                    }
                    int recordsTotal = employeeVM.TasksGridVMList.Count();
                    int pendingCount = employeeVM.TasksGridVMList.Where(x => x.Status != "Completed").Count();
                    int notStartedCount = employeeVM.TasksGridVMList.Where(x => x.Status == "Not Started").Count();
                    int completedCount = employeeVM.TasksGridVMList.Where(x => x.Status == "Completed").Count();
                    int onHoldCount = employeeVM.TasksGridVMList.Where(x => x.Status == "On Hold").Count();
                    int cancelledCount = employeeVM.TasksGridVMList.Where(x => x.Status == "Cancelled").Count();
                    int progressCount = employeeVM.TasksGridVMList.Where(x => x.Status == "In Progress").Count();

                    return Json(new
                    {
                        draw = "1",
                        pendingCount = pendingCount,
                        notStartedCount = notStartedCount,
                        completedCount = completedCount,
                        onHoldCount = onHoldCount,
                        cancelledCount = cancelledCount,
                        progressCount = progressCount,
                        recordsFiltered = recordsTotal,
                        recordsTotal = recordsTotal,
                        data = employeeVM.TasksGridVMList
                    });

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult LoadEmployeeFollowUps(string userName)
        {
            try
            {
                if (!String.IsNullOrEmpty(userName))
                {
                    currentUserName = userName;
                }
                TaskFollowUpMailBoxViewModel taskFollowUpMailBoxVM = new TaskFollowUpMailBoxViewModel();
                taskFollowUpMailBoxVM.InBoxFollowUpList = new List<TaskFollowUpInboxViewModel>();
                var connectionString = _iconfiguration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlQuery = 
                        "select TaskFollowUps.LastUpdatedAt," +
                        "Tasks.Title, " +
                        "(select Employees.UserCode from Employees where Id = TaskFollowUps.FollowerId) as FollowUpFrom," +
                        "(select Employees.EmployeeName from Employees where Id = TaskFollowUps.FollowerId) as FollowUpEmployeeName," +
                        "(select Employees.AvatarImage from Employees where Id = TaskFollowUps.FollowerId) as AvatarImage," +
                        "(select [Status] from TaskfollowUpStatus where TaskfollowUpStatus.Id = TaskFollowUps.StatusId) as [Status]," +
                        "Tasks.Id  from TaskFollowUps" +
                        " inner join TaskEmployees on TaskEmployees.TaskId = TaskFollowUps.TaskId" +
                        " inner join Employees on Employees.Id = TaskEmployees.EmployeeId" +
                        " inner join Tasks on Tasks.Id = TaskEmployees.TaskId" +
                        " and TaskFollowUps.StatusId = " + ((int)Common.Common.TaskFollowUpStatus.Open).ToString() +
                        " and Tasks.IsDeleted = 0" +
                        " and TaskEmployees.IsActive = 1" +
                        " and Employees.UserCode = '" + currentUserName + "'" +
                        " and TaskEmployees.TaskCapacityId = " + ((int)Common.Common.TaskCapacity.Assignee).ToString() +
                        " union" +
                        " select TaskFollowUps.LastUpdatedAt," +
                        "Tasks.Title, " +
                        "(select Employees.UserCode from Employees where Id = TaskFollowUps.FollowerId) as FollowUpFrom," +
                        "(select Employees.EmployeeName from Employees where Id = TaskFollowUps.FollowerId) as FollowUpEmployeeName," +
                        "(select Employees.AvatarImage from Employees where Id = TaskFollowUps.FollowerId) as AvatarImage," +
                        "(select [Status] from TaskfollowUpStatus where TaskfollowUpStatus.Id = TaskFollowUps.StatusId) as [Status]," +
                        "Tasks.Id  from TaskFollowUps" +
                        " inner join TaskFollowUpResponses on TaskFollowUpResponses.TaskFollowUpId = TaskFollowUps.Id" +
                        " inner join Employees on Employees.Id = TaskFollowUpResponses.RespondedById" +
                        " inner join Tasks on Tasks.Id = TaskFollowUps.TaskId" +
                        " where TaskFollowUps.StatusId = " + ((int)Common.Common.TaskFollowUpStatus.Close).ToString() +
                        " and Tasks.IsDeleted = 0" +
                        " and Employees.UserCode = '" + currentUserName + "'";
                    conn.Open();
                    var filteredTaskInbox = conn.Query(sqlQuery);
                    foreach (var inbox in filteredTaskInbox)
                    {
                        taskFollowUpMailBoxVM.InBoxFollowUpList.Add(
                      new TaskFollowUpInboxViewModel()
                      {
                          UpdatedDate = inbox.LastUpdatedAt.ToString("dd-MMM-yyyy"),
                          TaskInfo = inbox.Title,
                          FollowUpEmployeeName = inbox.FollowUpEmployeeName,
                          Status = inbox.Status,
                          TaskId = inbox.Id,
                          SortId = GetSortOrderOfStatus(inbox.Status),
                          AvatarImage = inbox.AvatarImage
                      });
                    }

                    int recordsTotal = filteredTaskInbox.Count();
                    int pendingFollowUps = filteredTaskInbox.Where(x => x.Status != "Close").Count();
                    int completedFollowUps = filteredTaskInbox.Where(x => x.Status == "Close").Count();

                    return Json(new
                    {
                        draw = "1",
                        pendingFollowUps = pendingFollowUps,
                        completedFollowUps = completedFollowUps,
                        recordsFiltered = recordsTotal,
                        recordsTotal = recordsTotal,
                        data = taskFollowUpMailBoxVM.InBoxFollowUpList
                    });

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult GetEmployeeDetails(string userName)
        {
            userName = userName == null ? currentUserName : userName;
            EmployeeViewModel employeeVM = new EmployeeViewModel();

            Employee thisEmployee = _context.Employees.Include(k => k.Department)
                .Where(x => x.UserCode == userName).SingleOrDefault();

            List<Department> departments = _context.Departments.Include(x => x.Manager).ToList();

            if (thisEmployee != null)
            {
                employeeVM.EmployeeName = userName;
                employeeVM.EmailAddres = thisEmployee.EmailAddress;
                employeeVM.Phone = thisEmployee.PhoneNo;
                employeeVM.Presence = "Present"; //insert attendance here
                employeeVM.Department = thisEmployee.Department.Name;
                employeeVM.Title = thisEmployee.JobTitle;
                employeeVM.AvatarImage = thisEmployee.AvatarImage;

                var employeeManager = (from c in _context.Employees
                                       join o in _context.Departments
                                       on c.Department.Id equals o.Id
                                       where c.UserCode == userName
                                       select new
                                       {
                                           UserName = o.ParentDepartment == null ? "" :
                                           c.Department.Manager.Id == c.Id ? o.ParentDepartment.Manager.UserCode
                                           : c.Department.Manager.UserCode
                                       }
                                        );


                if (employeeManager != null && employeeManager.Any())
                {
                    employeeVM.ReportsTo = employeeManager.FirstOrDefault().UserName;
                }
            }
            else
            {
                employeeVM.EmployeeName = string.Empty;
                employeeVM.AvatarImage = string.Empty;
            }

            return new JsonResult(new { employeeName = employeeVM.EmployeeName, avatarImage = employeeVM.AvatarImage });

        }



        public IActionResult Index(string userName)
        {
            userName = userName == null ? currentUserName : userName;
            EmployeeViewModel employeeVM = new EmployeeViewModel();
            
            Employee thisEmployee = _context.Employees.Include(k => k.Department)
                .Where(x => x.UserCode == userName).SingleOrDefault();
           
            List<Department> departments = _context.Departments.Include(x => x.Manager).ToList();

            employeeVM.EmployeeName = thisEmployee.EmployeeName;
            employeeVM.EmailAddres = thisEmployee.EmailAddress;
            employeeVM.Phone = thisEmployee.PhoneNo;
            employeeVM.Presence = "Present"; //insert attendance here
            employeeVM.Department = thisEmployee.Department.Name;
            employeeVM.Title = thisEmployee.JobTitle;
            employeeVM.AvatarImage = thisEmployee.AvatarImage;

            var employeeManager = (from c in _context.Employees
                                    join o in _context.Departments
                                    on c.Department.Id equals o.Id
                                    where c.UserCode == userName
                                    select new
                                    {
                                        UserName = o.ParentDepartment == null ? "" :
                                        c.Department.Manager.Id == c.Id ? o.ParentDepartment.Manager.UserCode
                                        : c.Department.Manager.UserCode
                                    }
                                    );


            if (employeeManager != null && employeeManager.Any())
            {
                employeeVM.ReportsTo = employeeManager.FirstOrDefault().UserName;
            }

            ViewData["UserName"] = userName;
            return PartialView("Index", employeeVM);
        }
    }
}