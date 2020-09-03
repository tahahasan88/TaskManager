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
        int currentTaskId = 4;

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
                .Where(x => x.Task.Id == currentTaskId)
                .ToListAsync();
            foreach (TaskEmployee employee in employees)
            {
                taskEmployeeVm.Add(new TaskEmployeeViewModel() { TaskEmployee = employee });
            }

            return View(taskEmployeeVm);
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

        // POST: TaskEmployees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskEmployee = await _context.TaskEmployees.FindAsync(id);
            _context.TaskEmployees.Remove(taskEmployee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskEmployeeExists(int id)
        {
            return _context.TaskEmployees.Any(e => e.Id == id);
        }
    }
}
