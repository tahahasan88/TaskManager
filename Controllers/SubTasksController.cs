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

        public async Task<IActionResult> SubTaskList(int id)
        {
            List<SbTaskViewModel> subTaskVMList = new List<SbTaskViewModel>();
            List<SubTask> subTasks = await _context.SubTasks
                .Include(x => x.TaskStatus)
                .Include(x => x.Task)
                .Where(x => x.IsDeleted == false && x.Task.Id == id)
                .ToListAsync();

            foreach (SubTask subTask in subTasks)
            {
                subTaskVMList.Add(new SbTaskViewModel()
                {
                    TaskId = subTask.Id,
                    Description = subTask.Description,
                    SubTaskAssignee = subTask.SubTaskAssigneeUserName,
                    IsCompleted = IsSubTaskCompleted(subTask.TaskStatus.Id)
                });
            }
            return new JsonResult(new { records = subTaskVMList });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string description, int taskId, bool isCompleted, string assignee)
        {
            bool subTaskExist = false;
            bool subTaskAdded = false;
            SubTask existingSubTask = _context.SubTasks.Where(x => x.Task.Id == taskId && x.Description == description && x.IsDeleted == false).SingleOrDefault();
            if (existingSubTask == null)
            {

                Data.Task thisTask = _context.Tasks.Where(X => X.Id == currentTaskId).SingleOrDefault();
                SubTask subTask = new SubTask();
                subTask.Description = description;
                subTask.Task = _context.Tasks.Where(X => X.Id == currentTaskId).SingleOrDefault();
                if (isCompleted)
                {
                    subTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.Completed).SingleOrDefault();
                }
                else
                {
                    subTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.NotStarted).SingleOrDefault();
                }
                subTask.LastUpdatedAt = DateTime.Now;
                subTask.LastUpdatedBy = currentUserName;
                if (assignee.Equals(string.Empty))
                {
                    TaskEmployee taskEmployee = _context.TaskEmployees.Where(x => x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee).FirstOrDefault();
                    subTask.SubTaskAssigneeUserName = taskEmployee.UserName;
                }
                else
                {
                    subTask.SubTaskAssigneeUserName = assignee;
                }

                TaskAudit progressAudit = new TaskAudit();
                progressAudit.ActionDate = DateTime.Now;
                progressAudit.ActionBy = currentUserName;
                progressAudit.Description = "Sub task assigned to " + subTask.SubTaskAssigneeUserName;
                progressAudit.Task = thisTask;
                progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.SubTasks).SingleOrDefault();
                _context.Add(progressAudit);

                _context.Add(subTask);
                await _context.SaveChangesAsync();
                subTaskAdded = true;
            }
            else
            {
                subTaskExist = true;
            }

            return new JsonResult(new { subTaskExist = subTaskExist, subTaskAdded = subTaskAdded });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, string description, int taskId, string assignee, bool isCompleted)
        {
            bool subTaskExist = false;
            bool subTaskUpdated = false;
            try
            {
                SubTask existingSubTask = _context.SubTasks.Where(x => x.Task.Id != taskId && x.Description == description && x.IsDeleted == false).SingleOrDefault();
                if (existingSubTask == null)
                {
                    SubTask subTask = _context.SubTasks.Where(X => X.Id == id).SingleOrDefault();
                    if (isCompleted)
                    {
                        subTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.Completed).SingleOrDefault();
                    }

                    subTask.LastUpdatedAt = DateTime.Now;
                    subTask.LastUpdatedBy = currentUserName;
                    subTask.SubTaskAssigneeUserName = assignee;
                    _context.Update(subTask);
                    await _context.SaveChangesAsync();
                    subTaskUpdated = true; 
                }
                else
                {
                    subTaskExist = true;
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return new JsonResult(new { subTaskExist = subTaskExist, subTaskUpdated = subTaskUpdated });
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

        public IActionResult IsSubTaskAlreadyExist(string subTaskDescription)
        {
            bool isSubTaskAlreadyAdded = false;
            Data.SubTask existingSubTask = _context.SubTasks.Where(x => x.Description == subTaskDescription).SingleOrDefault();
            isSubTaskAlreadyAdded = existingSubTask != null ? true : false;
            return new JsonResult(new { isSubTaskAlreadyAdded = isSubTaskAlreadyAdded });
        }

        public IActionResult DeleteSubTask(int subTaskId)
        {
            bool isSubTaskDeleted = false;
            Data.SubTask existingSubTask = _context.SubTasks.Where(x => x.Id == subTaskId).SingleOrDefault();
            if (existingSubTask != null)
            {
                existingSubTask.IsDeleted = true;
                //_context.SubTasks.Remove(subTask);
                _context.SubTasks.Attach(existingSubTask);
                _context.Entry(existingSubTask).State = EntityState.Modified;
                _context.SaveChangesAsync();
                isSubTaskDeleted = true;
            }
            return new JsonResult(new { isSubTaskAlreadyAdded = isSubTaskDeleted });
        }


        private bool SubTaskExists(int id)
        {
            return _context.SubTasks.Any(e => e.Id == id);
        }
    }
}
