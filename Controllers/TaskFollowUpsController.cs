using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class TaskFollowUpsController : Controller
    {
        private readonly TaskManagerContext _context;
        private int currentTask = 4;

        public TaskFollowUpsController(TaskManagerContext context)
        {
            _context = context;
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

        public bool IsUserAlreadyFollowing(string userName, int taskId)
        {
            var followUp = _context.TaskFollowUps.Where(x => x.FollowerUserName == userName && x.Task.Id == taskId).SingleOrDefault();
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
                    int taskFollowerCapacity = (int)TaskManager.Common.Common.TaskCapacity.Follower;
                    taskCreator.IsActive = true;
                    taskCreator.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskFollowerCapacity).SingleOrDefault();
                    taskCreator.UserName = currentUserName;
                    _context.Add(taskCreator);
                }

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
