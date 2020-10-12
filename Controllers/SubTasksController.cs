using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManager.Data;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class SubTasksController : BaseController
    {
        public int currentTaskId = 4;

        public SubTasksController(TaskManagerContext context, IConfiguration configuration) : base(context, configuration)
        {
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
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
            SetUserName();
            SubTaskListViewModel subTaskListViewModel = new SubTaskListViewModel();
            List<TaskEmployeeListViewModel> emmployeeVMList = new List<TaskEmployeeListViewModel>();
            List<SbTaskViewModel> subTaskVMList = new List<SbTaskViewModel>();

            List<SubTask> subTasks = await _context.SubTasks
                .Include(x => x.TaskStatus)
                .Include(x => x.Task)
                .Where(x => x.IsDeleted == false && x.Task.Id == id)
                .ToListAsync();

            List<Employee> employees = _context.Employees.ToList();
            
            TaskEmployee taskEmployee = _context.TaskEmployees
                       .Include(x => x.Employee)
                       .Where(x => x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee
                   && x.Task.Id == id && x.IsActive == true).FirstOrDefault();

            List<TaskEmployee> subTaskEmployees = _context.TaskEmployees
                    .Include(x => x.Employee)
                    .Where(x => x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.SubTaskAssignee
                && x.Task.Id == id && x.IsActive == true).ToList();

            subTaskListViewModel.employeeVMList = this.LoadSubTaskAssigneeDrpDwnData(currentUserName, taskEmployee.UserName, subTaskEmployees.Select(x=>x.UserName).ToList());

            foreach (SubTask subTask in subTasks)
            {
                subTaskVMList.Add(new SbTaskViewModel()
                {
                    TaskId = subTask.Task.Id,
                    SubTaskId = subTask.Id,
                    Description = subTask.Description,
                    SubTaskUserName = subTask.SubTaskAssigneeUserName,
                    SubTaskEmployeeName = employees.Where(x => x.UserCode == subTask.SubTaskAssigneeUserName).SingleOrDefault().EmployeeName,
                });
            }

            subTaskListViewModel.subTaskVMList = subTaskVMList;
            return new JsonResult(new { records = subTaskListViewModel });
        }



        [HttpPost]
        public async Task<IActionResult> Create(string description, int taskId, bool isCompleted, string assignee)
        {
            bool subTaskExist = false;
            bool subTaskAdded = false;
            SetUserName();
            SubTask existingSubTask = _context.SubTasks.Where(x => x.Task.Id == taskId && x.Description == description && x.IsDeleted == false).SingleOrDefault();
            var employees = _context.Employees.ToList();
            if (existingSubTask == null)
            {
                Data.Task thisTask = _context.Tasks.Where(X => X.Id == taskId).SingleOrDefault();
                SubTask subTask = new SubTask();
                subTask.Description = description;
                subTask.Task = thisTask;
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
                if (assignee == null || assignee == "")
                {
                    TaskEmployee taskEmployee = _context.TaskEmployees
                        .Include(x => x.Employee)
                        .Where(x => x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee
                    && x.Task.Id == taskId && x.IsActive == true).FirstOrDefault();
                    subTask.SubTaskAssigneeUserName = employees.Where(x => x.UserCode == taskEmployee.UserName).SingleOrDefault().UserCode;

                    TaskEmployee employee = _context.TaskEmployees.Where(x => x.UserName == subTask.SubTaskAssigneeUserName
                    && x.Task.Id == taskId
                    && x.TaskCapacity.Id == (int)TaskManager.Common.Common.TaskCapacity.SubTaskAssignee).FirstOrDefault();
                    if (employee == null)
                    {
                        TaskEmployee subTaskAssignee = new TaskEmployee();
                        subTaskAssignee.Task = subTask.Task;
                        int subTaskAssigneeCapacity = (int)TaskManager.Common.Common.TaskCapacity.SubTaskAssignee;
                        subTaskAssignee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == subTaskAssigneeCapacity).SingleOrDefault();
                        subTaskAssignee.UserName = subTask.SubTaskAssigneeUserName;
                        subTaskAssignee.IsActive = true;
                        _context.Add(subTaskAssignee);
                    }
                    else
                    {
                        employee.IsActive = true;
                        _context.TaskEmployees.Attach(employee);
                        _context.Entry(employee).State = EntityState.Modified;
                    }
                }

                TaskAudit progressAudit = new TaskAudit();
                progressAudit.ActionDate = DateTime.Now;
                progressAudit.ActionBy = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault().EmployeeName;
                progressAudit.Description = "Sub task created with description " + subTask.Description + " and assigned to " + employees.Where(x => x.UserCode == subTask.SubTaskAssigneeUserName).SingleOrDefault().EmployeeName;
                progressAudit.Task = thisTask;
                progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.SubTasks).SingleOrDefault();
                _context.Add(progressAudit);

                if (assignee != null && assignee != "")
                {
                    TaskEmployee employee = _context.TaskEmployees.Where(x => x.UserName == assignee
                    && x.Task.Id == taskId
                    && x.TaskCapacity.Id == (int)TaskManager.Common.Common.TaskCapacity.SubTaskAssignee
                    ).FirstOrDefault();
                    if (employee == null)
                    {
                        TaskEmployee subTaskAssignee = new TaskEmployee();
                        subTaskAssignee.Task = subTask.Task;
                        int subTaskAssigneeCapacity = (int)TaskManager.Common.Common.TaskCapacity.SubTaskAssignee;
                        subTaskAssignee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == subTaskAssigneeCapacity).SingleOrDefault();
                        subTaskAssignee.UserName = assignee;
                        subTaskAssignee.IsActive = true;
                        _context.Add(subTaskAssignee);
                    }
                    else
                    {
                        employee.IsActive = true;
                        _context.TaskEmployees.Attach(employee);
                        _context.Entry(employee).State = EntityState.Modified;
                    }
                    subTask.SubTaskAssigneeUserName = assignee;
                }

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


        [HttpPost, ActionName("Update")]
        public IActionResult Update(int id, string description, int taskId, string assignee, bool isCompleted)
        {
            bool subTaskExist = false;
            bool subTaskUpdated = false;
            try
            {
                SetUserName();
                SubTask existingSubTask = _context.SubTasks
                    .Include(x => x.Task)
                    .Where(x => x.Task.Id == taskId && x.Id != id && x.Description == description && x.IsDeleted == false).SingleOrDefault();
                if (existingSubTask == null)
                {
                    var employees = _context.Employees.ToList();
                    SubTask subTask = _context.SubTasks.Include(x => x.TaskStatus)
                        .Include(x => x.Task)
                        .Where(X => X.Id == id).SingleOrDefault();
                    string previousDescription = subTask.Description;
                    string previousAssignee = subTask.SubTaskAssigneeUserName;
                    Common.Common.TaskStatus previousStatus = (Common.Common.TaskStatus)subTask.TaskStatus.Id;

                    if (isCompleted)
                    {
                        subTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.Completed).SingleOrDefault();
                    }
                    else
                    {
                        subTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.NotStarted).SingleOrDefault();
                    }
                    subTask.Description = description;
                    subTask.LastUpdatedAt = DateTime.Now;
                    subTask.LastUpdatedBy = currentUserName;
                    subTask.SubTaskAssigneeUserName = assignee;
                    _context.Update(subTask);

                    if (previousDescription != description)
                    {
                        TaskAudit descriptionAudit = new TaskAudit();
                        descriptionAudit.ActionDate = DateTime.Now;
                        descriptionAudit.ActionBy = employees.Where(x => x.UserCode == currentUserName).SingleOrDefault().EmployeeName;
                        descriptionAudit.Description = "Sub task updated. Description changed from " + previousDescription + " to " + description;
                        descriptionAudit.Task = subTask.Task;
                        descriptionAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.SubTasks).SingleOrDefault();
                        _context.Add(descriptionAudit);
                    }
                    if (previousAssignee != assignee)
                    {
                        TaskAudit assigneeAudit = new TaskAudit();
                        assigneeAudit.ActionDate = DateTime.Now;
                        assigneeAudit.ActionBy = employees.Where(x => x.UserCode == currentUserName).SingleOrDefault().EmployeeName;
                        assigneeAudit.Description = "Sub task with description " + subTask.Description + 
                            " assigned to " + employees.Where(x => x.UserCode == assignee).SingleOrDefault().EmployeeName +
                            " from " + employees.Where(x => x.UserCode == previousAssignee).SingleOrDefault().EmployeeName; 

                        assigneeAudit.Task = subTask.Task;
                        assigneeAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.SubTasks).SingleOrDefault();
                        _context.Add(assigneeAudit);

                        TaskEmployee existingSubTaskEmployee = _context.TaskEmployees.Where(x => x.UserName == previousAssignee
                            && x.Task.Id == taskId
                            && x.TaskCapacity.Id == (int)TaskManager.Common.Common.TaskCapacity.SubTaskAssignee
                            ).FirstOrDefault();

                        int userSubTaskCount = _context.SubTasks.Where(x => x.Task.Id == taskId && x.IsDeleted == false
                            && x.SubTaskAssigneeUserName == previousAssignee).Count();

                        if (existingSubTaskEmployee != null && userSubTaskCount <= 1)
                        {
                            existingSubTaskEmployee.IsActive = false;
                            _context.TaskEmployees.Attach(existingSubTaskEmployee);
                            _context.Entry(existingSubTaskEmployee).State = EntityState.Modified;
                        }
                    }
                    if ( (isCompleted && previousStatus != Common.Common.TaskStatus.Completed)
                        || (!isCompleted && previousStatus == Common.Common.TaskStatus.Completed)
                        )
                    {
                        TaskAudit progressAudit = new TaskAudit();
                        progressAudit.ActionDate = DateTime.Now;
                        string previousStatusDescription = ((int)previousStatus) == 1 ? "Not Completed" : "Completed";
                        string modifiedStatusDescription = subTask.TaskStatus.Id == 1 ? "Not Completed" : "Completed";

                        progressAudit.ActionBy = employees.Where(x => x.UserCode == currentUserName).SingleOrDefault().EmployeeName;
                        progressAudit.Description = "Sub task with description " + subTask.Description + "  status updated from " +
                             previousStatusDescription  + " to " + modifiedStatusDescription;
                        progressAudit.Task = subTask.Task;
                        progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.SubTasks).SingleOrDefault();
                        _context.Add(progressAudit);
                    }

                    TaskEmployee employee = _context.TaskEmployees.Where(x => x.UserName == assignee
                    && x.Task.Id == taskId
                    && x.TaskCapacity.Id == (int)TaskManager.Common.Common.TaskCapacity.SubTaskAssignee
                    ).FirstOrDefault();
                    if (employee == null)
                    {
                        TaskEmployee taskAssignee = new TaskEmployee();
                        taskAssignee.Task = subTask.Task;
                        int subTaskAssigneeCapacity = (int)TaskManager.Common.Common.TaskCapacity.SubTaskAssignee;
                        taskAssignee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == subTaskAssigneeCapacity).SingleOrDefault();
                        taskAssignee.UserName = assignee;
                        taskAssignee.IsActive = true;
                        _context.Add(taskAssignee);
                    }
                    else
                    {
                        employee.IsActive = true;
                        _context.TaskEmployees.Attach(employee);
                        _context.Entry(employee).State = EntityState.Modified;
                    }

                    _context.SaveChanges();
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
            List<TaskEmployee> filteredTaskEmployees = _context.TaskEmployees.Where(x => x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Creator
                || x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee && x.Task.Id == currentTaskId).ToList();

            List<Employee> employeeList = _context.Employees.ToList();
            foreach (Employee employee in employeeList)
            {
                if (filteredTaskEmployees.Where(x => x.UserName == employee.UserName).Count() == 0)
                {
                    employeeDrpDwnList.Add(new SelectListItem() { Text = employee.EmployeeName, Value = employee.UserName });
                }
            }
            subTaskViewModel.EmployeeList = employeeDrpDwnList;
            if (!subTaskUserName.Equals(string.Empty))
            {
                subTaskViewModel.SelectedEmployee = employeeList.Where(x => x.UserName == subTaskUserName).FirstOrDefault().EmployeeName;
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
        public async Task<IActionResult> Create2(SubTaskViewModel subTaskVM)
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
                string employeeName = _context.Employees.Where(x => x.UserCode == subTaskVM.SubTask.SubTaskAssigneeUserName).SingleOrDefault().EmployeeName;
                progressAudit.Description = "Sub task " + subTaskVM.SubTask.Description + " assigned to " + employeeName;
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

        public IActionResult IsSubTaskAlreadyExist(string subTaskDescription)
        {
            bool isSubTaskAlreadyAdded = false;
            Data.SubTask existingSubTask = _context.SubTasks.Where(x => x.Description == subTaskDescription).SingleOrDefault();
            isSubTaskAlreadyAdded = existingSubTask != null ? true : false;
            return new JsonResult(new { isSubTaskAlreadyAdded = isSubTaskAlreadyAdded });
        }

        [HttpPost, ActionName("DeleteSubTask")]
        public IActionResult DeleteSubTask(int subTaskId)
        {
            SetUserName();
            bool isSubTaskDeleted = false;
            Data.SubTask existingSubTask = _context.SubTasks
                .Include(x => x.Task)
                .Where(x => x.Id == subTaskId).SingleOrDefault();
            if (existingSubTask != null)
            {

                existingSubTask.IsDeleted = true;
                existingSubTask.LastUpdatedAt = DateTime.Now;
                existingSubTask.LastUpdatedBy = currentUserName;
                _context.SubTasks.Attach(existingSubTask);
                _context.Entry(existingSubTask).State = EntityState.Modified;

                TaskAudit deleteAudit = new TaskAudit();
                deleteAudit.ActionDate = DateTime.Now;
                deleteAudit.ActionBy = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault().EmployeeName;
                deleteAudit.Description = "Sub task with description " + existingSubTask.Description + " deleted";
                deleteAudit.Task = existingSubTask.Task;
                deleteAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.SubTasks).SingleOrDefault();
                _context.Add(deleteAudit);

                TaskEmployee taskEmployee = _context.TaskEmployees
                    .Where(x => x.Task.Id == existingSubTask.Task.Id
                    && x.TaskCapacity.Id == (int)TaskManager.Common.Common.TaskCapacity.SubTaskAssignee
                    && x.UserName == existingSubTask.SubTaskAssigneeUserName
                    ).FirstOrDefault();

                int userSubTaskCount = _context.SubTasks.Where(x => x.Task.Id == existingSubTask.Task.Id && x.IsDeleted == false
                           && x.SubTaskAssigneeUserName == existingSubTask.SubTaskAssigneeUserName
                           ).Count();

                if (taskEmployee != null && userSubTaskCount <= 1)
                {
                    taskEmployee.IsActive = false;
                    _context.TaskEmployees.Attach(taskEmployee);
                    _context.Entry(taskEmployee).State = EntityState.Modified;
                }

                _context.SaveChanges();
                isSubTaskDeleted = true;
            }
            return new JsonResult(new { isSubTaskDeleted = isSubTaskDeleted });
        }


        private bool SubTaskExists(int id)
        {
            return _context.SubTasks.Any(e => e.Id == id);
        }
    }
}
