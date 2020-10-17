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
    public class TaskEmployeesController : Controller
    {
        private readonly TaskManagerContext _context;

        public TaskEmployeesController(TaskManagerContext context)
        {
            _context = context;
        }

        // GET: TaskEmployees
        public async Task<IActionResult> Index()
        {
            List<TaskEmployeeViewModel> taskEmployeeVm = new List<TaskEmployeeViewModel>();
            List<TaskEmployee> employees =  await _context.TaskEmployees
                .Include(x => x.TaskCapacity)
                .Include(x => x.Task)
                .Where(x => x.Task.Id == 4)
                .ToListAsync();
            foreach (TaskEmployee employee in employees)
            {
                taskEmployeeVm.Add(new TaskEmployeeViewModel() { TaskEmployee = employee });
            }

            return View(taskEmployeeVm);
        }

        public async Task<IActionResult> List(int taskId)
        {
            List<TaskEmployeeListViewModel> emmployeeVMList = new List<TaskEmployeeListViewModel>();
            List<TaskEmployee> employeeList = await _context.TaskEmployees
                .Include(x => x.Task)
                .Include(x => x.TaskCapacity)
                .Where(x => x.Task.Id == taskId && x.IsActive == true)
                .ToListAsync();
            var allEmployees = _context.Employees.ToList();

            foreach (TaskEmployee employee in employeeList)
            {
                emmployeeVMList.Add(new TaskEmployeeListViewModel()
                {
                    UserName = employee.Employee.UserCode,
                    CapacityId = employee.TaskCapacity.Id,
                    IsActive = employee.IsActive,
                    EmployeeName = allEmployees.Where(x => x.UserCode == employee.Employee.UserCode).SingleOrDefault().EmployeeName
                });
            }
            return new JsonResult(new { records = emmployeeVMList });
        }

        public IActionResult AllEmployees()
        {
            List<TaskEmployeeListViewModel> emmployeeVMList = new List<TaskEmployeeListViewModel>();
            List<Employee> employeeList = _context.Employees.ToList();

            foreach (Employee employee in employeeList)
            {
                emmployeeVMList.Add(new TaskEmployeeListViewModel()
                {
                    UserName = employee.UserCode,
                    EmailAddress = employee.EmailAddress,
                    EmployeeName = employee.EmployeeName
                });
            }
            return new JsonResult(new { records = emmployeeVMList });
        }


        public IActionResult IsActionPermissible(string userName,int taskId, int actionId)
        {
             TaskEmployee employeeProfile = _context.TaskEmployees
                .Include(x => x.Task)
                .Include(x => x.TaskCapacity)
                .Where(x => x.Task.Id == taskId && x.Employee.UserCode == userName)
                .FirstOrDefault();

            bool isAllowed = TaskPermissions.IsAllowed((Common.Common.TaskAction)actionId,(Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
            return new JsonResult(new { isAllowed = isAllowed });
        }

        public IActionResult GetUserPermissions(string userName, int taskId)
        {
            List<TaskEmployee> employeeProfiles = _context.TaskEmployees
               .Include(x => x.Task)
               .Include(x => x.TaskCapacity)
               .Where(x => x.Task.Id == taskId && x.Employee.UserCode == userName && x.IsActive == true)
               .OrderBy(x => x.TaskCapacity)
               .ToList();

            if (!employeeProfiles.Any())
            {
                employeeProfiles.Add(new TaskEmployee() { TaskCapacity = new TaskCapacity() { Id = (int)Common.Common.TaskCapacity.ExTaskAssignee } });
            }
            bool isTaskDeletionAllowed = false;
            bool isSubTaskDeletionAllowed = false;
            bool isTaskEditAllowed = false;
            bool isSubTaskEditAllowed = false;
            bool isProgressUpdateAllowed = false;

            foreach (TaskEmployee employeeProfile in employeeProfiles)
            {
                if (!isTaskDeletionAllowed)
                {
                    isTaskDeletionAllowed = TaskPermissions.IsAllowed(Common.Common.TaskAction.TaskDelete, (Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
                }
                if (!isSubTaskDeletionAllowed)
                {
                    isSubTaskDeletionAllowed = TaskPermissions.IsAllowed(Common.Common.TaskAction.SubTaskDelete, (Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
                }
                if (!isTaskEditAllowed)
                {
                    isTaskEditAllowed = TaskPermissions.IsAllowed(Common.Common.TaskAction.TaskEdit, (Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
                }
                if (!isSubTaskEditAllowed)
                {
                    isSubTaskEditAllowed = TaskPermissions.IsAllowed(Common.Common.TaskAction.SubTaskEdit, (Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
                }
                if (!isProgressUpdateAllowed)
                {
                    isProgressUpdateAllowed = TaskPermissions.IsAllowed(Common.Common.TaskAction.ProgressUpdate, (Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
                }
            }

            if (isSubTaskEditAllowed)
            {
                isSubTaskEditAllowed = IsTaskInLockedState(taskId) == true ? false : true;
            }
            
            return new JsonResult(new { isTaskDeletionAllowed = isTaskDeletionAllowed, isSubTaskDeletionAllowed  = isSubTaskDeletionAllowed , isTaskEditAllowed  = isTaskEditAllowed,
                isSubTaskEditAllowed = isSubTaskEditAllowed,
                isProgressUpdateAllowed = isProgressUpdateAllowed
            });
        }


        private bool IsTaskInLockedState(int taskId)
        {
            return _context.Tasks.Where(x => x.Id == taskId && (x.TaskStatus.Id == (int)Common.Common.TaskStatus.Completed
            || x.TaskStatus.Id == (int)Common.Common.TaskStatus.Cancelled
            || x.TaskStatus.Id == (int)Common.Common.TaskStatus.OnHold)).Any();
        }

        // GET: TaskEmployees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskEmployee = await _context.TaskEmployees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskEmployee == null)
            {
                return NotFound();
            }

            return View(taskEmployee);
        }

        // GET: TaskEmployees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaskEmployees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,IsActive")] TaskEmployee taskEmployee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskEmployee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskEmployee);
        }

        // GET: TaskEmployees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskEmployee = await _context.TaskEmployees.FindAsync(id);
            if (taskEmployee == null)
            {
                return NotFound();
            }
            return View(taskEmployee);
        }

        // POST: TaskEmployees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,IsActive")] TaskEmployee taskEmployee)
        {
            if (id != taskEmployee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskEmployee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskEmployeeExists(taskEmployee.Id))
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
            return View(taskEmployee);
        }

        // GET: TaskEmployees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskEmployee = await _context.TaskEmployees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskEmployee == null)
            {
                return NotFound();
            }

            return View(taskEmployee);
        }


        private bool TaskEmployeeExists(int id)
        {
            return _context.TaskEmployees.Any(e => e.Id == id);
        }
    }
}
