using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Common;
using TaskManager.Data;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskManagerContext _context;
        private string currentUserName;

        public TasksController(TaskManagerContext context)
        {
            _context = context;
            currentUserName = "tahahasan";
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tasks.Where(x => x.IsDeleted != true).ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            TaskViewModel taskViewModel = new TaskViewModel();
            EmployeeList employeeList = new EmployeeList();
            popuLateTaskViewDropDowns(taskViewModel, employeeList);
            return View(taskViewModel);
        }

        private void popuLateTaskViewDropDowns(TaskViewModel taskVm, EmployeeList employeeList)
        {
            List<SelectListItem> employeeDrpDwnList = new List<SelectListItem>();
            List<SelectListItem> taskPriorityDrpDwnList = new List<SelectListItem>();
            var taskPriorityEnumsList = Enum.GetValues(typeof(Common.Common.TaskPriority));
            foreach (Common.Common.TaskPriority priority in taskPriorityEnumsList)
            {
                taskPriorityDrpDwnList.Add(new SelectListItem() { Text = priority.ToString(), Value = ((int)priority).ToString() });
            }
            taskVm.PriorityList = taskPriorityDrpDwnList;

            foreach (Employee employee in employeeList.Employees)
            {
                if (employee.UserName != currentUserName)
                {
                    employeeDrpDwnList.Add(new SelectListItem() { Text = employee.EmployeeName, Value = employee.UserName.ToString() });
                }
            }
            taskVm.EmployeeList = employeeDrpDwnList;
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel taskVM)
        {
            if (ModelState.IsValid)
            {

                taskVM.Task.CreatedAt = DateTime.Now;
                taskVM.Task.LastUpdatedAt = DateTime.Now;
                taskVM.Task.LastUpdatedBy = currentUserName;
                taskVM.Task.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.NotStarted).SingleOrDefault();
                taskVM.Task.TaskPriority = _context.TaskPriority.Where(x => x.Id == taskVM.PriorityId).SingleOrDefault();
                _context.Add(taskVM.Task);
                await _context.SaveChangesAsync();

                TaskManager.Data.Task Thistask = _context.Tasks.Where(x => x.Id == taskVM.Task.Id).SingleOrDefault();
                //Add task assignee
                TaskEmployee taskAssignee = new TaskEmployee();
                taskAssignee.Task = Thistask;
                int taskAssigneeCapacity = (int)TaskManager.Common.Common.TaskCapacity.Assignee;
                taskAssignee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskAssigneeCapacity).SingleOrDefault();
                taskAssignee.UserName = taskVM.AssigneeCode;
                _context.Add(taskAssignee);

                //Add task creator
                TaskEmployee taskCreator = new TaskEmployee();
                taskCreator.Task = Thistask;
                int taskCreatorCapacity = (int)TaskManager.Common.Common.TaskCapacity.Creator;
                taskCreator.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskCreatorCapacity).SingleOrDefault();
                taskCreator.UserName = currentUserName;
                _context.Add(taskCreator);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(taskVM);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(x => x.TaskPriority)
                .Include(x => x.TaskStatus)
                .Where(x => x.Id == id).SingleOrDefaultAsync();

            if (task == null)
            {
                return NotFound();
            }
            TaskViewModel taskVM = new TaskViewModel();
            taskVM.Task = task;
            popuLateTaskViewDropDowns(taskVM, new EmployeeList());
            taskVM.PriorityId = task.TaskPriority.Id;
            taskVM.AssigneeCode = task.LastUpdatedBy;

            return View(taskVM);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskManager.Web.Models.TaskViewModel taskVM, string command)
        {
            if (id != taskVM.Task.Id)
            {
                return NotFound();
            }
            if (command.Equals("Delete"))
            {
                return RedirectToAction("Delete", "Tasks", new { id = id });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TaskEmployee currentEmployee = _context.TaskEmployees
                        .Include(x => x.TaskCapacity)
                        .Where(x => x.UserName == currentUserName).SingleOrDefault();
                    if (TaskPermissions.IsAllowed(Common.Common.TaskAction.TaskEdit, (Common.Common.TaskCapacity)currentEmployee.TaskCapacity.Id))
                    {
                        taskVM.Task.LastUpdatedAt = DateTime.Now;
                        taskVM.Task.LastUpdatedBy = currentUserName;

                        _context.Update(taskVM.Task);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(taskVM.Task.Id))
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
            return View(taskVM);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            TaskEmployee currentEmployee = _context.TaskEmployees
              .Include(x => x.TaskCapacity)
              .Where(x => x.UserName == currentUserName).SingleOrDefault();

            if (TaskPermissions.IsAllowed(Common.Common.TaskAction.TaskDelete, (Common.Common.TaskCapacity)currentEmployee.TaskCapacity.Id))
            {
                //_context.Tasks.Remove(task);
                task.IsDeleted = true;
                _context.Tasks.Attach(task);
                _context.Entry(task).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
