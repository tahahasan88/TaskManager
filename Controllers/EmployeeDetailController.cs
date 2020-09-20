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
    public class EmployeeDetailController : Controller
    {
        public string currentUserName = "";
        private readonly TaskManagerContext _context;


        public EmployeeDetailController(TaskManagerContext context, IConfiguration configuration)
        {
            _context = context;
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
            
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

                var taskEmployees = (from e in _context.TaskEmployees
                .Where(x => x.IsActive == true && x.UserName == userName)
                                     join t in _context.Tasks
                                     .Where(x => x.IsDeleted == false)
                                     on e.Task.Id equals t.Id
                                     select new
                                     {
                                         e.UserName,
                                         t.TaskStatus.Status,
                                         t.TaskProgress,
                                         t.Title,
                                         t.LastUpdatedAt,
                                         t.Target,
                                         t.Id,
                                         SortId = GetSortOrderOfStatus(t.TaskStatus.Id) 
                                     })
                .OrderByDescending(m => m.LastUpdatedAt);

                foreach (var taskEmployee in taskEmployees)
                {
                    employeeVM.TasksGridVMList.Add(new TasksGridViewModel()
                    {
                        Title = taskEmployee.Title,
                        Status = taskEmployee.Status,
                        TargetDate = taskEmployee.Target.ToString("dd-MMM-yyyy HH:mm:ss"),
                        TaskId = taskEmployee.Id,
                        SortId = taskEmployee.SortId
                    });
                }
                int recordsTotal = taskEmployees.Count();
                int pendingCount = taskEmployees.Where(x => x.Status != "Completed").Count();
                int notStartedCount = taskEmployees.Where(x => x.Status == "Not Started").Count();
                int completedCount = taskEmployees.Where(x => x.Status == "Completed").Count();
                int onHoldCount = taskEmployees.Where(x => x.Status == "On Hold").Count();
                int cancelledCount = taskEmployees.Where(x => x.Status == "Cancelled").Count();
                int progressCount = taskEmployees.Where(x => x.Status == "In Progress").Count();

                //Returning Json Data
                return Json(new { draw = "1",
                    pendingCount = pendingCount,
                    notStartedCount = notStartedCount,
                    completedCount = completedCount,
                    onHoldCount = onHoldCount,
                    cancelledCount = cancelledCount,
                    progressCount = progressCount,
                    recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = employeeVM.TasksGridVMList });
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


        public async Task<IActionResult> Index(string userName)
        {
            userName = userName == null ? currentUserName : userName;
            EmployeeViewModel employeeVM = new EmployeeViewModel();
            
            EmployeeList employeeList = new EmployeeList();
            InternalEmployee thisEmployee = employeeList.Employees.Where(x => x.UserName == userName).SingleOrDefault();
            employeeVM.EmployeeName = userName;
            employeeVM.EmailAddres = thisEmployee.EmailAddress;
            employeeVM.Phone = thisEmployee.PhoneNo;
            employeeVM.Presence = "Present";
            employeeVM.ReportsTo = "Haroon";

            ViewData["UserName"] = userName;
            return PartialView("Index", employeeVM);
        }
    }
}