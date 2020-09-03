﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Common;
using TaskManager.Data;
using TaskManager.Web.Models;
using static TaskManager.Common.Common;

namespace TaskManager.Web.Controllers
{
    public class TaskFollowUpsController : BaseController
    {
        private int currentTask = 4;
        public string currentUserName = "Hasan";

        public TaskFollowUpsController(TaskManagerContext context) : base(context)
        {
        }

        // GET: TaskFollowUps
        public async Task<IActionResult> Index()
        {
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
                                 .Where(x=>x.UserName == currentUserName)
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
            taskFollowUpMailBoxVM.OutBoxFollowUpList = new List<TaskFollowUpOutboxViewModel>();

            foreach (var inbox in filteredTaskInbox)
            {
                taskFollowUpMailBoxVM.InBoxFollowUpList.Add(
                    new TaskFollowUpInboxViewModel()
                    {
                        Remarks = inbox.Remarks,
                        UpdatedDate = inbox.LastUpdatedAt,
                        TaskInfo = inbox.Id.ToString(),
                        FollowUpFrom = inbox.FollowerUserName,
                        Status = inbox.Status
                    }); 
            }
            var filteredTaskOutbox =  await _context.TaskFollowUps
                .Include(x => x.Task)
                .Include(x => x.Task.TaskStatus)
                .Where(x => x.FollowerUserName == currentUserName).ToListAsync();
            foreach (var outbox in filteredTaskOutbox)
            {
                taskFollowUpMailBoxVM.OutBoxFollowUpList.Add(
                    new TaskFollowUpOutboxViewModel()
                    {
                        FollowUpDate = outbox.LastUpdatedAt,
                        Remarks = outbox.Remarks,
                        TaskInfo = outbox.Task.Id.ToString(),
                        Status = outbox.Task.TaskStatus.Status
                    }
                );
            }

            return View(taskFollowUpMailBoxVM);
        }

        public bool IsUserAlreadyFollowing(string userName, int taskId)
        {
            var followUp = _context.TaskFollowUps.Where(x => x.FollowerUserName == userName && x.Task.Id == taskId).FirstOrDefault();
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
            return View(taskFollowUpVm);
        }

        // POST: TaskFollowUps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskFollowUpViewModel taskFollowUpVM)
        {
            string currentUserName = "tahahasan";
            if (ModelState.IsValid)
            {
                TaskManager.Data.Task Thistask = _context.Tasks.Where(x => x.Id == currentTask).SingleOrDefault();
                taskFollowUpVM.TaskFollowUp.LastUpdatedAt = DateTime.Now;
                taskFollowUpVM.TaskFollowUp.CreatedAt = DateTime.Now;
                taskFollowUpVM.TaskFollowUp.FollowerUserName = currentUserName;
                taskFollowUpVM.TaskFollowUp.Task = Thistask;
                _context.Add(taskFollowUpVM.TaskFollowUp);

                if (!IsUserAlreadyFollowing(currentUserName, currentTask))
                {
                    TaskEmployee taskCreator = new TaskEmployee();
                    taskCreator.Task = Thistask;
                    
                    int taskFollowerCapacity = (int)Common.Common.TaskCapacity.Follower;
                    taskCreator.IsActive = true;
                    taskCreator.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskFollowerCapacity).SingleOrDefault();
                    taskCreator.UserName = currentUserName;
                    _context.Add(taskCreator);
                }

                TaskAudit followUpAudit = new TaskAudit();
                followUpAudit.ActionDate = DateTime.Now;
                followUpAudit.ActionBy = currentUserName;
                followUpAudit.Description = "Follow up requested";
                followUpAudit.Task = Thistask;
                followUpAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.FollowUp).SingleOrDefault();
                _context.Add(followUpAudit);

                List<string> employeeList = GetTaskEmployeeEmailAddresses(currentTask, currentUserName);
                await MailSystem.SendEmail(MailSystem.RequestFollowUpEmailTemplate, employeeList);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskFollowUpVM);
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

        // POST: TaskFollowUps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskFollowUp = await _context.TaskFollowUps.FindAsync(id);
            _context.TaskFollowUps.Remove(taskFollowUp);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskFollowUpExists(int id)
        {
            return _context.TaskFollowUps.Any(e => e.Id == id);
        }
    }
}
