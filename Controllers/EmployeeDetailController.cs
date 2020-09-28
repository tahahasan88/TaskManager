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
            
            if ((Common.Common.TaskStatus)(statusId) == Common.Common.TaskStatus.Completed){
                return 5;
            }
            if ((Common.Common.TaskStatus)(statusId) == Common.Common.TaskStatus.OnHold)
            {
                return 4;
            }
            if ((Common.Common.TaskStatus)(statusId) == Common.Common.TaskStatus.Cancelled)
            {
                return 3;
            }
            if ((Common.Common.TaskStatus)(statusId) == Common.Common.TaskStatus.NotStarted)
            {
                return 2;
            }
            if ((Common.Common.TaskStatus)(statusId) == Common.Common.TaskStatus.InProgress)
            {
                return 1;
            }
            return 1;
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
                    "TaskEmployees.UserName," +
                    "TaskStatus.Status," +
                    "Tasks.TaskProgress," +
                    "Tasks.Title," +
                    "Tasks.LastUpdatedAt," +
                    "Tasks.Target," +
                    "TaskStatus.Id as TaskStatusId" +
                    " from TaskEmployees" +
                    " inner join Tasks on Tasks.Id = TaskEmployees.TaskId" +
                    " inner join TaskStatus on TaskStatus.Id = Tasks.TaskStatusId" +
                    " where TaskEmployees.UserName = '"+ userName + "'" +
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
                userName = userName == null ? currentUserName : userName;
                var filteredTaskInbox = (from c in _context.TaskFollowUps
                                         join o in _context.TaskEmployees
                                        .Where(x => x.UserName == userName &&
                                          x.IsActive == true && x.Task.IsDeleted == false)
                                         on c.Task.Id equals o.Task.Id
                                         select new
                                         {
                                             c.FollowerUserName,
                                             c.LastUpdatedAt,
                                             c.Remarks,
                                             c.Task.Title,
                                             c.Task.TaskStatus.Status,
                                             c.Task.Id,
                                             SortId = GetSortOrderOfStatus(c.Task.TaskStatus.Id)
                                         })
                                .Where(x => x.FollowerUserName != userName)
                                .OrderByDescending(m => m.LastUpdatedAt);

                TaskFollowUpMailBoxViewModel taskFollowUpMailBoxVM = new TaskFollowUpMailBoxViewModel();
                taskFollowUpMailBoxVM.InBoxFollowUpList = new List<TaskFollowUpInboxViewModel>();
                foreach (var inbox in filteredTaskInbox)
                {
                    taskFollowUpMailBoxVM.InBoxFollowUpList.Add(
                        new TaskFollowUpInboxViewModel()
                        {
                            Remarks = inbox.Remarks,
                            UpdatedDate = inbox.LastUpdatedAt.ToString("dd-MMM-yyyy"),
                            TaskInfo = inbox.Title,
                            FollowUpFrom = inbox.FollowerUserName,
                            Status = inbox.Status,
                            TaskId = inbox.Id,
                            SortId = inbox.SortId
                        });
                }


                int recordsTotal = filteredTaskInbox.Count();
                int pendingFollowUps = filteredTaskInbox.Where(x => x.Status != "Completed").Count();
                int completedFollowUps = filteredTaskInbox.Where(x => x.Status == "Completed").Count();
                //Returning Json Data
                return Json(new { draw = "1",
                    pendingFollowUps = pendingFollowUps,
                    completedFollowUps = completedFollowUps,
                    recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = taskFollowUpMailBoxVM.InBoxFollowUpList });
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public IActionResult Index(string userName)
        {
            userName = userName == null ? currentUserName : userName;
            EmployeeViewModel employeeVM = new EmployeeViewModel();
            
            Employee thisEmployee = _context.Employees.Include(k => k.Department)
                .Where(x => x.UserName == userName).SingleOrDefault();
           
            List<Department> departments = _context.Departments.Include(x => x.Manager).ToList();

            employeeVM.EmployeeName = userName;
            employeeVM.EmailAddres = thisEmployee.EmailAddress;
            employeeVM.Phone = thisEmployee.PhoneNo;
            employeeVM.Presence = "Present"; //insert attendance here

            var employeeManager = (from c in _context.Employees
                                    join o in _context.Departments
                                    on c.Department.Id equals o.Id
                                    where c.UserName == userName
                                    select new
                                    {
                                        UserName = o.ParentDepartment == null ? "" :
                                        c.Department.Manager.Id == c.Id ? o.ParentDepartment.Manager.UserName
                                        : c.Department.Manager.UserName
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