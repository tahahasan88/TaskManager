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

        public IActionResult LoadEmployeeTasksData()
        {
            try
            {
                EmployeeViewModel employeeVM = new EmployeeViewModel();
                employeeVM.TasksGridVMList = new List<TasksGridViewModel>();

                var taskEmployees = (from e in _context.TaskEmployees
                .Where(x => x.IsActive == true && x.UserName == currentUserName)
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
                                         t.Target
                                     })
                .OrderByDescending(m => m.LastUpdatedAt);

                foreach (var taskEmployee in taskEmployees)
                {
                    employeeVM.TasksGridVMList.Add(new TasksGridViewModel()
                    {
                        Title = taskEmployee.Title,
                        Status = taskEmployee.Status,
                        TargetDate = taskEmployee.Target.ToString("dd-MMM-yyyy HH:mm:ss")
                    });
                }
                int recordsTotal = taskEmployees.Count();
                //Returning Json Data
                return Json(new { draw = "1", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = employeeVM.TasksGridVMList });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult LoadEmployeeFollowUps()
        {
            try
            {
                var filteredTaskInbox = (from c in _context.TaskFollowUps
                                         join o in _context.TaskEmployees
                                         .Where(x => x.UserName == currentUserName)
                                         .Where(x => x.Task.IsDeleted == false)
                                         on c.Task.Id equals o.Task.Id
                                         select new
                                         {
                                             c.FollowerUserName,
                                             c.LastUpdatedAt,
                                             c.Remarks,
                                             c.Task.Id,
                                             c.Task.TaskStatus.Status
                                         })
                                .Where(x => x.FollowerUserName != currentUserName)
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
                            TaskInfo = inbox.Id.ToString(),
                            FollowUpFrom = inbox.FollowerUserName,
                            Status = inbox.Status
                        });
                }


                int recordsTotal = filteredTaskInbox.Count();
                //Returning Json Data
                return Json(new { draw = "1", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = taskFollowUpMailBoxVM.InBoxFollowUpList });
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IActionResult> Index()
        {
            EmployeeViewModel employeeVM = new EmployeeViewModel();
            EmployeeList employeeList = new EmployeeList();
            Employee thisEmployee = employeeList.Employees.Where(x => x.EmployeeName == currentUserName).SingleOrDefault();
            employeeVM.EmployeeName = currentUserName;
            employeeVM.EmailAddres = thisEmployee.EmailAddress;
            employeeVM.Phone = thisEmployee.PhoneNo;
            employeeVM.Presence = "Present";
            employeeVM.ReportsTo = "Haroon";

            ViewData["UserName"] = currentUserName;
            return PartialView("Index", employeeVM);
        }
    }
}