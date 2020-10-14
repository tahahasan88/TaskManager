﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManager.Common;
using TaskManager.Data;
using TaskManager.Web.Models;
using static TaskManager.Common.Common;

namespace TaskManager.Web.Controllers
{
    public class TaskFollowUpsController : BaseController
    {
        public TaskFollowUpsController(TaskManagerContext context, IConfiguration configuration) : base(context, configuration)
        {
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
        }

        // GET: TaskFollowUps
        public async Task<IActionResult> Index1()
        {
            return View(await _context.TaskFollowUps.ToListAsync());
        }

        public async Task<IActionResult> Index()
        {
            SetUserName();
            ViewData["UserName"] = currentUserName;
            Employee employee = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
            ViewData["EmployeeName"] = employee == null ? "" : employee.EmployeeName;
            return View(await _context.TaskFollowUps.ToListAsync());
        }


        // GET: TaskFollowUps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskFollowUp = await _context.TaskFollowUps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskFollowUp == null)
            {
                return NotFound();
            }

            return View(taskFollowUp);
        }

        public async Task<IActionResult> FollowUpMailBox()
        {
            var filteredTaskInbox = (from c in _context.TaskFollowUps
                                 join o in _context.TaskEmployees
                                 .Where(x=>x.Employee.UserCode == currentUserName)
                                 .Where(x => x.Task.IsDeleted == false)
                                 on c.Task.Id equals o.Task.Id
                                 select new
                                 {
                                     c.Follower.UserCode,
                                     c.LastUpdatedAt,
                                     c.Remarks,
                                     c.Task.Id,
                                     c.Task.TaskStatus.Status
                                 })
                                 .Where(x => x.UserCode != currentUserName)
                                 .OrderByDescending(m => m.LastUpdatedAt);
            TaskFollowUpMailBoxViewModel taskFollowUpMailBoxVM = new TaskFollowUpMailBoxViewModel();
            taskFollowUpMailBoxVM.InBoxFollowUpList = new List<TaskFollowUpInboxViewModel>();
            taskFollowUpMailBoxVM.OutBoxFollowUpList = new List<TaskFollowUpOutboxViewModel>();

            foreach (var inbox in filteredTaskInbox)
            {
                taskFollowUpMailBoxVM.InBoxFollowUpList.Add(
                    new TaskFollowUpInboxViewModel()
                    {
                        Remarks = inbox.Remarks,
                        UpdatedDate = inbox.LastUpdatedAt.ToString("dd-MMM-yyyy"),
                        TaskInfo = inbox.Id.ToString(),
                        FollowUpFrom = inbox.UserCode,
                        Status = inbox.Status
                    }); 
            }
            var filteredTaskOutbox =  await _context.TaskFollowUps
                .Include(x => x.Task)
                .Include(x => x.Task.TaskStatus)
                .Where(x => x.Follower.UserCode == currentUserName).ToListAsync();
            foreach (var outbox in filteredTaskOutbox)
            {
                taskFollowUpMailBoxVM.OutBoxFollowUpList.Add(
                    new TaskFollowUpOutboxViewModel()
                    {
                        FollowUpDate = outbox.LastUpdatedAt.ToString("dd-MMM-yyyy"),
                        Remarks = outbox.Remarks,
                        TaskInfo = outbox.Task.Id.ToString(),
                        Status = outbox.Task.TaskStatus.Status
                    }
                );
            }

            return View(taskFollowUpMailBoxVM);
        }

        public IActionResult LoadTaskInBoxData(string username)
        {
            try
            {
                if (!String.IsNullOrEmpty(username))
                {
                    currentUserName = username;
                }
                var filteredTaskInbox = (from c in _context.TaskFollowUps
                                         join o in _context.TaskEmployees
                                         .Where(x => x.Employee.UserCode == currentUserName &&
                                          x.IsActive == true && x.Task.IsDeleted == false)
                                         on c.Task.Id equals o.Task.Id
                                         select new
                                         {
                                             c.Follower.UserCode,
                                             c.LastUpdatedAt,
                                             c.Remarks,
                                             c.Task.Title,
                                             c.Task.TaskStatus.Status,
                                             c.Task.Id
                                         })
                                 .Where(x => x.UserCode != currentUserName)
                                 .OrderByDescending(m => m.LastUpdatedAt);

                int recordsTotal = filteredTaskInbox.Count();
                var employees = _context.Employees.ToList();

                List<TaskFollowUpInboxViewModel> taskFollowUpInboxVMList = new List<TaskFollowUpInboxViewModel>();

                foreach (var inbox in filteredTaskInbox)
                {
                    taskFollowUpInboxVMList.Add(
                        new TaskFollowUpInboxViewModel()
                        {
                            Remarks = inbox.Remarks,
                            UpdatedDate = inbox.LastUpdatedAt.ToString("dd-MMM-yyyy"),
                            TaskInfo = inbox.Title,
                            FollowUpFrom = inbox.UserCode,
                            FollowUpEmployeeName = employees.Where(x => x.UserCode == inbox.UserCode).SingleOrDefault().EmployeeName,
                            Status = inbox.Status,
                            TaskId = inbox.Id
                        }); ;
                }

                //Returning Json Data
                return Json(new { draw = "1", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = taskFollowUpInboxVMList });

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public IActionResult LoadTaskOutBoxData(string username)
        {
            try
            {
                if (!String.IsNullOrEmpty(username))
                {
                    currentUserName = username;
                }
                var filteredTaskOutbox = (from c in _context.TaskFollowUps
                                          join o in _context.TaskEmployees
                                          .Where(x => x.IsActive == true && x.Task.IsDeleted == false)
                                          on c.Task.Id equals o.Task.Id
                                          select new
                                          {
                                              c.Follower.UserCode,
                                              c.LastUpdatedAt,
                                              c.Remarks,
                                              c.Task.Title,
                                              c.Task.TaskStatus.Status,
                                              c.Task.Id
                                          })
                                 .Where(x => x.UserCode == currentUserName)
                                 .Distinct()
                                 .OrderByDescending(m => m.LastUpdatedAt);

                int recordsTotal = filteredTaskOutbox.Count();

                List<TaskFollowUpOutboxViewModel> taskFollowUpOutBoxVMList = new List<TaskFollowUpOutboxViewModel>();

                foreach (var outbox in filteredTaskOutbox)
                {
                    taskFollowUpOutBoxVMList.Add(
                        new TaskFollowUpOutboxViewModel()
                        {
                            FollowUpDate = outbox.LastUpdatedAt.ToString("dd-MMM-yyyy"),
                            Remarks = outbox.Remarks,
                            TaskInfo = outbox.Title,
                            Status = outbox.Status,
                            TaskId = outbox.Id
                        }
                    );
                }

                //Returning Json Data
                return Json(new { draw = "1", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = taskFollowUpOutBoxVMList });

            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public bool IsUserAlreadyFollowing(string userName, int taskId)
        {
            var followUp = _context.TaskFollowUps.Where(x => x.Follower.UserCode == userName && x.Task.Id == taskId).FirstOrDefault();
            if (followUp != null)
            {
                return true;
            }
            else
                return false;
        }

        // GET: TaskFollowUps/Create
        public IActionResult Create()
        {
            TaskFollowUpViewModel taskFollowUpVm = new TaskFollowUpViewModel();
            return PartialView("_Create", taskFollowUpVm);
        }

        // POST: TaskFollowUps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskFollowUpViewModel taskFollowUpVM)
        {
            SetUserName();
            if (ModelState.IsValid)
            {
                string [] listOfTaskIds = taskFollowUpVM.ListofTasks.Split(",");
                Dictionary<int, List<string>> taskEmployeeDictionary = new Dictionary<int, List<string>>();

                Employee currentEmployee = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
                foreach (string taskId in listOfTaskIds)
                {
                    int thisTaskId = Convert.ToInt32(taskId);
                    TaskManager.Data.Task Thistask = _context.Tasks.Where(x => x.Id == thisTaskId).SingleOrDefault();
                    TaskFollowUp followUp = new TaskFollowUp();

                    followUp.LastUpdatedAt = DateTime.Now;
                    followUp.CreatedAt = DateTime.Now;
                    followUp.Follower = currentEmployee;
                    followUp.Task = Thistask;
                    followUp.Remarks = taskFollowUpVM.Remarks;
                    _context.Add(followUp);

                    if (!IsUserAlreadyFollowing(currentUserName, thisTaskId))
                    {
                        TaskEmployee taskCreator = new TaskEmployee();
                        taskCreator.Task = Thistask;

                        int taskFollowerCapacity = (int)Common.Common.TaskCapacity.Follower;
                        taskCreator.IsActive = true;
                        taskCreator.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskFollowerCapacity).SingleOrDefault();
                        taskCreator.Employee = currentEmployee;
                        _context.Add(taskCreator);
                    }

                    TaskAudit followUpAudit = new TaskAudit();
                    followUpAudit.ActionDate = DateTime.Now;
                    followUpAudit.ActionBy = currentEmployee;
                    followUpAudit.Description = "Follow up requested for Task with title " + Thistask.Title + " and remarks are " + followUp.Remarks;
                    followUpAudit.Task = Thistask;
                    followUpAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.FollowUp).SingleOrDefault();
                    _context.Add(followUpAudit);

                    List<string> employeeList = GetTaskEmployeeEmailAddresses(thisTaskId, currentUserName);
                    taskEmployeeDictionary.Add(thisTaskId, employeeList);
                }
                await _context.SaveChangesAsync();
                //foreach (KeyValuePair<int, List<string>> keyValuePair in taskEmployeeDictionary)
                //{
                //    await MailSystem.SendEmail(MailSystem.RequestFollowUpEmailTemplate, keyValuePair.Value);
                //}

                //return RedirectToAction(nameof(Index));
            }
            return PartialView("_Create", taskFollowUpVM);
        }

        // GET: TaskFollowUps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskFollowUp = await _context.TaskFollowUps.FindAsync(id);
            if (taskFollowUp == null)
            {
                return NotFound();
            }
            return View(taskFollowUp);
        }

        // POST: TaskFollowUps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Remarks,FollowerUserName,LastUpdatedAt,CreatedAt")] TaskFollowUp taskFollowUp)
        {
            if (id != taskFollowUp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskFollowUp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskFollowUpExists(taskFollowUp.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(taskFollowUp);
        }

        // GET: TaskFollowUps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskFollowUp = await _context.TaskFollowUps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskFollowUp == null)
            {
                return NotFound();
            }

            return View(taskFollowUp);
        }

        private bool TaskFollowUpExists(int id)
        {
            return _context.TaskFollowUps.Any(e => e.Id == id);
        }
    }
}
