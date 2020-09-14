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

            foreach (TaskEmployee employee in employeeList)
            {
                emmployeeVMList.Add(new TaskEmployeeListViewModel()
                {
                    UserName = employee.UserName,
                    CapacityId = employee.TaskCapacity.Id,
                    IsActive = employee.IsActive
                });
            }
            return new JsonResult(new { records = emmployeeVMList });
        }

        public IActionResult AllEmployees()
        {
            List<TaskEmployeeListViewModel> emmployeeVMList = new List<TaskEmployeeListViewModel>();
            EmployeeList employeeList = new EmployeeList();

            foreach (Employee employee in employeeList.Employees)
            {
                emmployeeVMList.Add(new TaskEmployeeListViewModel()
                {
                    UserName = employee.UserName,
                    EmailAddress = employee.EmailAddress
                });
            }
            return new JsonResult(new { records = emmployeeVMList });
        }


        public IActionResult IsActionPermissible(string userName,int taskId, int actionId)
        {
             TaskEmployee employeeProfile = _context.TaskEmployees
                .Include(x => x.Task)
                .Include(x => x.TaskCapacity)
                .Where(x => x.Task.Id == taskId && x.UserName == userName)
                .FirstOrDefault();

            bool isAllowed = TaskPermissions.IsAllowed((Common.Common.TaskAction)actionId,(Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
            return new JsonResult(new { isAllowed = isAllowed });
        }

        public IActionResult GetUserPermissions(string userName, int taskId)
        {
            TaskEmployee employeeProfile = _context.TaskEmployees
               .Include(x => x.Task)
               .Include(x => x.TaskCapacity)
               .Where(x => x.Task.Id == taskId && x.UserName == userName)
               .FirstOrDefault();
            

            bool isTaskDeletionAllowed = TaskPermissions.IsAllowed(Common.Common.TaskAction.TaskDelete, (Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
            bool isSubTaskDeletionAllowed = TaskPermissions.IsAllowed(Common.Common.TaskAction.SubTaskDelete, (Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
            bool isTaskEditAllowed = TaskPermissions.IsAllowed(Common.Common.TaskAction.TaskEdit, (Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);
            bool isSubTaskEditAllowed = TaskPermissions.IsAllowed(Common.Common.TaskAction.SubTaskEdit, (Common.Common.TaskCapacity)employeeProfile.TaskCapacity.Id);

            return new JsonResult(new { isTaskDeletionAllowed = isTaskDeletionAllowed, isSubTaskDeletionAllowed  = isSubTaskDeletionAllowed , isTaskEditAllowed  = isTaskEditAllowed,
                isSubTaskEditAllowed = isSubTaskEditAllowed });
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
