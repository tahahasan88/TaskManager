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
    public class SubTasksController : Controller
    {
        private readonly TaskManagerContext _context;
        public int currentTaskId = 4;
        public string currentUserName = "tahahasan";


        public SubTasksController(TaskManagerContext context)
        {
            _context = context;
        }


        private bool IsSubTaskCompleted(int taskStatusId)
        {
            return taskStatusId == (int)Common.Common.TaskStatus.Completed;
        }

        // GET: SubTasks
        public async Task<IActionResult> Index()
        {
            List<SubTaskViewModel> subTaskVMList = new List<SubTaskViewModel>();
            List<SubTask> subTasks = await _context.SubTasks
                .Include(x => x.TaskStatus)
                .Include(x => x.Task)
                .Where(x => x.IsDeleted == false && x.Task.Id == currentTaskId)
                .ToListAsync();
            foreach (SubTask subTask in subTasks)
            {
                subTaskVMList.Add(new SubTaskViewModel() { SubTask = subTask,
                    IsCompleted = IsSubTaskCompleted(subTask.TaskStatus.Id) });
            }
            return View(subTaskVMList);
        }

        // GET: SubTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subTask = await _context.SubTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subTask == null)
            {
                return NotFound();
            }

            return View(subTask);
        }

        private void PopulateSubTaskViewModel(SubTaskViewModel subTaskViewModel, string subTaskUserName = "")
        {
            List<SelectListItem> employeeDrpDwnList = new List<SelectListItem>();
            EmployeeList employeeList = new EmployeeList();
            List<TaskEmployee> filteredTaskEmployees = _context.TaskEmployees.Where(x => x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Creator
                || x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee && x.Task.Id == currentTaskId).ToList();

            foreach (Employee employee in employeeList.Employees)
            {
                if (filteredTaskEmployees.Where(x => x.UserName == employee.UserName).Count() == 0)
                {
                    employeeDrpDwnList.Add(new SelectListItem() { Text = employee.EmployeeName, Value = employee.UserName });
                }
            }
            subTaskViewModel.EmployeeList = employeeDrpDwnList;
            if (!subTaskUserName.Equals(string.Empty))
            {
                subTaskViewModel.SelectedEmployee = employeeList.Employees.Where(x => x.UserName == subTaskUserName).FirstOrDefault().EmployeeName;
            }
        }

        // GET: SubTasks/Create
        public IActionResult Create()
        {
            SubTaskViewModel subTaskViewModel = new SubTaskViewModel();
            PopulateSubTaskViewModel(subTaskViewModel);
            return View(subTaskViewModel);
        }

        // POST: SubTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubTaskViewModel subTaskVM)
        {
            if (ModelState.IsValid)
            {
                string description = subTaskVM.SubTask.Description;
                Data.Task thisTask = _context.Tasks.Where(X => X.Id == currentTaskId).SingleOrDefault();
                subTaskVM.SubTask = new SubTask();
                subTaskVM.SubTask.Description = description;
                subTaskVM.SubTask.Task = _context.Tasks.Where(X => X.Id == currentTaskId).SingleOrDefault();
                if (subTaskVM.IsCompleted)
                {
                    subTaskVM.SubTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.Completed).SingleOrDefault();
                }
                else
                {
                    subTaskVM.SubTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.NotStarted).SingleOrDefault();
                }
                subTaskVM.SubTask.LastUpdatedAt = DateTime.Now;
                subTaskVM.SubTask.LastUpdatedBy = currentUserName;
                subTaskVM.SubTask.SubTaskAssigneeUserName = subTaskVM.SelectedEmployee;

                TaskAudit progressAudit = new TaskAudit();
                progressAudit.ActionDate = DateTime.Now;
                progressAudit.ActionBy = currentUserName;
                progressAudit.Description = "Sub task assigned to " + subTaskVM.SubTask.SubTaskAssigneeUserName;
                progressAudit.Task = thisTask;
                progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.SubTasks).SingleOrDefault();
                _context.Add(progressAudit);

                _context.Add(subTaskVM.SubTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subTaskVM);
        }

        // GET: SubTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subTask = await _context.SubTasks.FindAsync(id);
            if (subTask == null)
            {
                return NotFound();
            }
            SubTaskViewModel subTaskViewModel = new SubTaskViewModel();
            PopulateSubTaskViewModel(subTaskViewModel, subTask.SubTaskAssigneeUserName);
            subTaskViewModel.SubTask = subTask;
            subTaskViewModel.IsCompleted = subTask.TaskStatus.Id == (int)TaskManager.Common.Common.TaskStatus.Completed;
            return View(subTaskViewModel);
        }

        // POST: SubTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubTaskViewModel subTaskVM)
        {
            if (id != subTaskVM.SubTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string description = subTaskVM.SubTask.Description;
                    subTaskVM.SubTask.Task = _context.Tasks.Where(X => X.Id == currentTaskId).SingleOrDefault();
                    if (subTaskVM.IsCompleted)
                    {
                        subTaskVM.SubTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.Completed).SingleOrDefault();
                    }
                    else
                    {
                        subTaskVM.SubTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.NotStarted).SingleOrDefault();
                    }
                    subTaskVM.SubTask.LastUpdatedAt = DateTime.Now;
                    subTaskVM.SubTask.LastUpdatedBy = currentUserName;
                    subTaskVM.SubTask.SubTaskAssigneeUserName = subTaskVM.SelectedEmployee;
                    _context.Update(subTaskVM.SubTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubTaskExists(subTaskVM.SubTask.Id))
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
            return View(subTaskVM);
        }

        // GET: SubTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subTask = await _context.SubTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subTask == null)
            {
                return NotFound();
            }

            return View(subTask);
        }

        // POST: SubTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subTask = await _context.SubTasks.FindAsync(id);
            subTask.IsDeleted = true;
            //_context.SubTasks.Remove(subTask);
            _context.SubTasks.Attach(subTask);
            _context.Entry(subTask).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubTaskExists(int id)
        {
            return _context.SubTasks.Any(e => e.Id == id);
        }
    }
}
