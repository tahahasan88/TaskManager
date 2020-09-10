using System;
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

namespace TaskManager.Web.Controllers
{
    public class TasksController : BaseController
    {
        private string currentUserName;

        public TasksController(TaskManagerContext context, IConfiguration configuration) : base(context)
        {
            currentUserName =configuration.GetSection("TaskManagerUserName").Value;
        }

        public IActionResult TasksSummary()
        {
            TaskSummaryViewModel taskSummaryVM = new TaskSummaryViewModel();
            taskSummaryVM.PendingTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
                != (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.IsDeleted != true
                && x.Target >= DateTime.Now).Count();

            taskSummaryVM.OverDueTasksCount = _context.Tasks.Where(x => x.TaskStatus.Id
                != (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.IsDeleted != true
                && DateTime.Now > x.Target).Count();

            taskSummaryVM.ResolvedTodayCount = _context.Tasks.Where(x => x.TaskStatus.Id
                == (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.IsDeleted != true
                && x.LastUpdatedAt.Date == DateTime.Now.Date).Count();

            taskSummaryVM.FollowUpsCount = _context.TaskFollowUps.Where(x => x.Task.TaskStatus.Id
                != (int)TaskManager.Common.Common.TaskStatus.Completed
                && x.Task.IsDeleted != true
                ).Count();

            return new JsonResult(new { records = taskSummaryVM });
        }


        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            if (TempData.ContainsKey("deletionId"))
            {
                TaskManager.Data.Task deletedTask = _context.Tasks.Where(x => x.Id == Convert.ToInt32(TempData["deletionId"])).SingleOrDefault();
                ViewData["deletionMessage"] = "Task with ID => " + deletedTask.Id + " has been deleted";
            }
            return View(await _context.Tasks.Where(x => x.IsDeleted != true).ToListAsync());
        }

        public IActionResult LoadTasksData()
        {
            try
            {
                var tasks = _context.Tasks
                            .Include(x => x.TaskStatus)
                            .Where(x => x.IsDeleted != true).OrderByDescending(x => x.LastUpdatedAt).ToList();

                //total number of rows count 
                int recordsTotal = tasks.Count;
                List<TasksGridViewModel> tasksGridVMList = new List<TasksGridViewModel>();
                foreach (TaskManager.Data.Task task in tasks)
                {
                    TasksGridViewModel taskGridVM = new TasksGridViewModel();

                    taskGridVM.TaskId = task.Id;
                    taskGridVM.Progress = task.TaskProgress;
                    taskGridVM.LastUpdated = task.LastUpdatedAt.ToString();
                    taskGridVM.Status = task.TaskStatus.Status;
                    taskGridVM.Title = task.Title;
                    TaskEmployee employee = _context.TaskEmployees.Where(x => x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee
                        && x.Task.Id == task.Id).FirstOrDefault();
                    taskGridVM.AssignedTo = employee.UserName;
                    tasksGridVMList.Add(taskGridVM);
                }
                //Returning Json Data
                return Json(new { draw = "1", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = tasksGridVMList });

            }
            catch (Exception ex)
            {
                throw;
            }

        }


        // GET: Tasks
        public async Task<IActionResult> List()
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

        public ActionResult Create()
        {
            TaskCreateViewModel taskVM = new TaskCreateViewModel();
            EmployeeList employeeList = new EmployeeList();
            popuLateTaskViewDropDowns(taskVM, employeeList);
            return PartialView("_Create", taskVM);
        }

        public ActionResult CreateMultiple()
        {
            TaskMultipleCreateViewModel taskVM = new TaskMultipleCreateViewModel();
            return PartialView("_CreateMultiple", taskVM);
        }

        private void popuLateTaskViewDropDowns(TaskCreateViewModel taskVm, EmployeeList employeeList)
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
            taskVm.Target = DateTime.Now;
        }

        private void popuLateTaskViewDropDowns2(TaskViewModel taskVm, EmployeeList employeeList)
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
  
        [HttpPost]
        public async Task<ActionResult> Create(TaskCreateViewModel taskCreateVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Data.Task task = new Data.Task();
                    task.IsDeleted = false;
                    task.LastUpdatedAt = DateTime.Now;
                    task.LastUpdatedBy = currentUserName;
                    task.CreatedAt = DateTime.Now;
                    task.Target = taskCreateVM.Target;
                    task.Title = taskCreateVM.Title;
                    task.Description = taskCreateVM.Description;
                    task.TaskProgress = "0";
                    task.TaskPriority = _context.TaskPriority.Where(x => x.Id == taskCreateVM.PriorityId).SingleOrDefault();
                    task.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.NotStarted).SingleOrDefault();
                    _context.Add(task);
                    await _context.SaveChangesAsync();

                    //Add task assignee
                    TaskEmployee taskAssignee = new TaskEmployee();
                    taskAssignee.Task = task;
                    int taskAssigneeCapacity = (int)TaskManager.Common.Common.TaskCapacity.Assignee;
                    taskAssignee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskAssigneeCapacity).SingleOrDefault();
                    taskAssignee.UserName = taskCreateVM.AssigneeCode;
                    _context.Add(taskAssignee);

                    //Add task creator
                    TaskEmployee taskCreator = new TaskEmployee();
                    taskCreator.Task = task;
                    int taskCreatorCapacity = (int)TaskManager.Common.Common.TaskCapacity.Creator;
                    taskCreator.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskCreatorCapacity).SingleOrDefault();
                    taskCreator.UserName = currentUserName;
                    _context.Add(taskCreator);

                    //Audit this task creation..
                    TaskAudit progressAudit = new TaskAudit();
                    progressAudit.ActionDate = DateTime.Now;
                    progressAudit.ActionBy = currentUserName;
                    progressAudit.Description = "Task created. Progess is 0%";
                    progressAudit.Task = task;
                    progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Progress).SingleOrDefault();
                    _context.Add(progressAudit);

                    TaskAudit assignmentAudit = new TaskAudit();
                    assignmentAudit.ActionDate = DateTime.Now;
                    assignmentAudit.ActionBy = currentUserName;
                    assignmentAudit.Description = "Task created. Assigned to " + taskCreateVM.AssigneeCode;
                    assignmentAudit.Task = task;
                    assignmentAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Assignment).SingleOrDefault();
                    _context.Add(assignmentAudit);
                    await _context.SaveChangesAsync();

                    EmployeeList employeeList = new EmployeeList();
                    popuLateTaskViewDropDowns(taskCreateVM, employeeList);
                }
                else
                {
                    EmployeeList employeeList = new EmployeeList();
                    popuLateTaskViewDropDowns(taskCreateVM, employeeList);
                }
            }
            catch (Exception ex)
            {
                
            }

            return PartialView("_Create", taskCreateVM);
        }



        [HttpPost]
        public async Task<ActionResult> CreateMultiple(TaskMultipleCreateViewModel taskCreateVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string[] listOfTaskTitles = taskCreateVM.ListOfTaskTitles.Split(",");
                    foreach (string title in listOfTaskTitles)
                    {
                        Data.Task task = new Data.Task();
                        task.IsDeleted = false;
                        task.LastUpdatedAt = DateTime.Now;
                        task.LastUpdatedBy = currentUserName;
                        task.CreatedAt = DateTime.Now;
                        task.Target = taskCreateVM.Target;
                        task.Title = title;
                        task.Description = taskCreateVM.Description;
                        task.TaskProgress = "0";
                        task.TaskPriority = _context.TaskPriority.Where(x => x.Id == taskCreateVM.PriorityId).SingleOrDefault();
                        task.TaskStatus = _context.TaskStatus.Where(x => x.Id == (int)TaskManager.Common.Common.TaskStatus.NotStarted).SingleOrDefault();
                        _context.Add(task);
                        await _context.SaveChangesAsync();

                        //Add task assignee
                        TaskEmployee taskAssignee = new TaskEmployee();
                        taskAssignee.Task = task;
                        int taskAssigneeCapacity = (int)TaskManager.Common.Common.TaskCapacity.Assignee;
                        taskAssignee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskAssigneeCapacity).SingleOrDefault();
                        taskAssignee.UserName = taskCreateVM.AssigneeCode;
                        _context.Add(taskAssignee);

                        //Add task creator
                        TaskEmployee taskCreator = new TaskEmployee();
                        taskCreator.Task = task;
                        int taskCreatorCapacity = (int)TaskManager.Common.Common.TaskCapacity.Creator;
                        taskCreator.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskCreatorCapacity).SingleOrDefault();
                        taskCreator.UserName = currentUserName;
                        _context.Add(taskCreator);

                        //Audit this task creation..
                        TaskAudit progressAudit = new TaskAudit();
                        progressAudit.ActionDate = DateTime.Now;
                        progressAudit.ActionBy = currentUserName;
                        progressAudit.Description = "Task created. Progess is 0%";
                        progressAudit.Task = task;
                        progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Progress).SingleOrDefault();
                        _context.Add(progressAudit);

                        TaskAudit assignmentAudit = new TaskAudit();
                        assignmentAudit.ActionDate = DateTime.Now;
                        assignmentAudit.ActionBy = currentUserName;
                        assignmentAudit.Description = "Task created. Assigned to " + taskCreateVM.AssigneeCode;
                        assignmentAudit.Task = task;
                        assignmentAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Assignment).SingleOrDefault();
                        _context.Add(assignmentAudit);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

            }

            return PartialView("_CreateMultiple", taskCreateVM);
        }



        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create1(TaskViewModel taskVM)
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

                //Audit this task creation..
                TaskAudit progressAudit = new TaskAudit();
                progressAudit.ActionDate = DateTime.Now;
                progressAudit.ActionBy = currentUserName;
                progressAudit.Description = "Task created. Progess is 0%";
                progressAudit.Task = Thistask;
                progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Progress).SingleOrDefault();
                _context.Add(progressAudit);

                TaskAudit assignmentAudit = new TaskAudit();
                assignmentAudit.ActionDate = DateTime.Now;
                assignmentAudit.ActionBy = currentUserName;
                assignmentAudit.Description = "Task created. Assigned to " + taskVM.AssigneeCode;
                assignmentAudit.Task = Thistask;
                assignmentAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Assignment).SingleOrDefault();
                _context.Add(assignmentAudit);

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
            popuLateTaskViewDropDowns2(taskVM, new EmployeeList());
            taskVM.PriorityId = task.TaskPriority.Id;
            TaskEmployee currentEmployee = _context.TaskEmployees.Where(x => x.Task.Id == id
                && x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee).FirstOrDefault();

            taskVM.AssigneeCode = currentEmployee.UserName;
            taskVM.CurrentUserName = currentUserName;
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
                    List<TaskEmployee> taskEmployees = await _context.TaskEmployees
                        .Include(x => x.TaskCapacity)
                        .Where(x => x.Task.Id == id).ToListAsync();

                    TaskEmployee existingEmployee= taskEmployees.Where(x => x.TaskCapacity.Id ==
                        (int)Common.Common.TaskCapacity.Assignee).SingleOrDefault();

                    if (taskVM.AssigneeCode != existingEmployee.UserName)
                    {
                        int exTaskAssigneeCapacity = (int)TaskManager.Common.Common.TaskCapacity.ExTaskAssignee;
                        existingEmployee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == exTaskAssigneeCapacity).SingleOrDefault();
                        _context.Update(existingEmployee);

                        TaskEmployee newEmployee = taskEmployees.Where(x => x.UserName ==
                            taskVM.AssigneeCode).SingleOrDefault();
                        TaskManager.Data.Task Thistask = _context.Tasks.Where(x => x.Id == id).SingleOrDefault();

                        int taskAssigneeCapacity = (int)TaskManager.Common.Common.TaskCapacity.Assignee;
                        if (newEmployee != null)
                        {
                            newEmployee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id ==taskAssigneeCapacity).SingleOrDefault();
                            _context.Update(newEmployee);
                        }
                        else
                        {
                            newEmployee = new TaskEmployee();
                            newEmployee.UserName = taskVM.AssigneeCode;
                            newEmployee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskAssigneeCapacity).SingleOrDefault();
                            newEmployee.Task = Thistask;
                            newEmployee.IsActive = true;
                            _context.Add(newEmployee);
                        }

                        TaskAudit progressAudit = new TaskAudit();
                        progressAudit.ActionDate = DateTime.Now;
                        progressAudit.ActionBy = currentUserName;
                        progressAudit.Description = "Assigned changed from " + existingEmployee.UserName + " to " + taskVM.AssigneeCode;
                        progressAudit.Task = Thistask;
                        progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Progress).SingleOrDefault();
                        _context.Add(progressAudit);
                    }

                    TaskEmployee currentEmployee = taskEmployees
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

        private void populateUpdateProgressVMDropDowns(UpdateProgressViewModel updateProgressVM)
        {
            List<SelectListItem> statusDrpDwnList = new List<SelectListItem>();
            var taskStatusEnumsList = Enum.GetValues(typeof(Common.Common.TaskPriority));
            foreach (Common.Common.TaskStatus taskStatus in taskStatusEnumsList)
            {
                statusDrpDwnList.Add(new SelectListItem() { Text = taskStatus.ToString(), Value = ((int)taskStatus).ToString() });
            }
            updateProgressVM.StatusList = statusDrpDwnList;
        }

        private void populateUpdateProgressVMDropDowns(TaskUpdateProgressViewModel updateProgressVM)
        {
            List<SelectListItem> statusDrpDwnList = new List<SelectListItem>();
            var taskStatusEnumsList = Enum.GetValues(typeof(Common.Common.TaskPriority));
            foreach (Common.Common.TaskStatus taskStatus in taskStatusEnumsList)
            {
                statusDrpDwnList.Add(new SelectListItem() { Text = taskStatus.ToString(), Value = ((int)taskStatus).ToString() });
            }
            updateProgressVM.StatusList = statusDrpDwnList;
        }

        public async Task<IActionResult> UpdateProgress(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(x => x.TaskStatus)
                .Where(x => x.Id == id).SingleOrDefaultAsync();

            if (task == null)
            {
                return NotFound();
            }
            UpdateProgressViewModel updateProgressVM = new UpdateProgressViewModel();
            updateProgressVM.Task = task;
            populateUpdateProgressVMDropDowns(updateProgressVM);
            return View(updateProgressVM);
        }



        public async Task<IActionResult> UpdateTaskProgress(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(x => x.TaskStatus)
                .Where(x => x.Id == id).SingleOrDefaultAsync();

            if (task == null)
            {
                return NotFound();
            }
            TaskUpdateProgressViewModel updateProgressVM = new TaskUpdateProgressViewModel();
            updateProgressVM.Id = task.Id;
            updateProgressVM.Remarks = task.Description;
            updateProgressVM.Status = task.TaskStatus.Status;
            updateProgressVM.TaskProgress = task.TaskProgress;
            populateUpdateProgressVMDropDowns(updateProgressVM);
            updateProgressVM.TaskEmployees = new List<string>();
            var taskEmployees = await _context.TaskEmployees
                .Where(x => x.Task.Id == id).ToListAsync();
            foreach (TaskEmployee employee in taskEmployees)
            {
                updateProgressVM.TaskEmployees.Add(employee.UserName);
            }
            return PartialView("_UpdateProgress", updateProgressVM);
        }


        public IActionResult DeleteRedirect(int id)
        {
            TempData["deletionId"] = id;
            return RedirectToAction("Index", "tasks");
        }

        [HttpPost, ActionName("UpdateTaskProgress")]
        public async Task<IActionResult> UpdateTaskProgress(int id, TaskUpdateProgressViewModel updateVM)
        {
            if (id != updateVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TaskEmployee currentEmployee = _context.TaskEmployees
                        .Include(x => x.TaskCapacity)
                        .Where(x => x.UserName == currentUserName && x.Task.Id == id).FirstOrDefault();
                    if (TaskPermissions.IsAllowed(Common.Common.TaskAction.ProgressUpdate, (Common.Common.TaskCapacity)currentEmployee.TaskCapacity.Id))
                    {
                        TaskManager.Data.Task thisTask = _context.Tasks.Where(x => x.Id == id).SingleOrDefault();

                        string previousProgress = thisTask.TaskProgress;
                        string taskProgress = updateVM.TaskProgress;
                        thisTask.LastUpdatedAt = DateTime.Now;
                        thisTask.LastUpdatedBy = currentUserName;
                        thisTask.TaskProgress = taskProgress;
                        thisTask.Description = updateVM.Remarks;
                        thisTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == Convert.ToInt32(updateVM.Status)).SingleOrDefault();
                        _context.Update(thisTask);

                        TaskFollowUpResponse taskFollowUpResponse = new TaskFollowUpResponse();
                        taskFollowUpResponse.RespondedBy = currentUserName;
                        taskFollowUpResponse.LastUpdatedAt = DateTime.Now;
                        taskFollowUpResponse.CreatedAt = DateTime.Now;
                        taskFollowUpResponse.TaskResponse = taskProgress;
                        taskFollowUpResponse.Task = thisTask;
                        _context.Add(taskFollowUpResponse);

                        TaskAudit followUpResponseAudit = new TaskAudit();
                        followUpResponseAudit.ActionDate = DateTime.Now;
                        followUpResponseAudit.ActionBy = currentUserName;
                        followUpResponseAudit.Description = "Follow up taken";
                        followUpResponseAudit.Task = thisTask;
                        followUpResponseAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.FollowUpResponse).SingleOrDefault();
                        _context.Add(followUpResponseAudit);

                        if (previousProgress != taskProgress)
                        {
                            //Audit this update task..
                            TaskAudit progressAudit = new TaskAudit();
                            progressAudit.ActionDate = DateTime.Now;
                            progressAudit.ActionBy = currentUserName;
                            progressAudit.Description = "Progress updated to " + taskProgress;
                            progressAudit.Task = thisTask;
                            progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Progress).SingleOrDefault();
                            _context.Add(progressAudit);
                        }

                        await _context.SaveChangesAsync();
                        List<string> employeeList = GetTaskEmployeeEmailAddresses(id, currentUserName);
                        await MailSystem.SendEmail(MailSystem.FollowUpResponseEmailTemplate, employeeList);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(updateVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            populateUpdateProgressVMDropDowns(updateVM);
            updateVM.TaskEmployees = new List<string>();
            var taskEmployees = await _context.TaskEmployees
                .Where(x => x.Task.Id == id).ToListAsync();
            foreach (TaskEmployee employee in taskEmployees)
            {
                updateVM.TaskEmployees.Add(employee.UserName);
            }
            return PartialView("_UpdateProgress", updateVM);
        }


        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("UpdateProgress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(int id, UpdateProgressViewModel updateVM)
        {
            if (id != updateVM.Task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    TaskEmployee currentEmployee = _context.TaskEmployees
                        .Include(x => x.TaskCapacity)
                        .Where(x => x.UserName == currentUserName).FirstOrDefault();
                    if (TaskPermissions.IsAllowed(Common.Common.TaskAction.ProgressUpdate, (Common.Common.TaskCapacity)currentEmployee.TaskCapacity.Id))
                    {
                        TaskManager.Data.Task thisTask = _context.Tasks.Where(x => x.Id == id).SingleOrDefault();
                        string previousProgress = thisTask.TaskProgress;
                        string taskProgress = updateVM.Task.TaskProgress;
                        updateVM.Task = thisTask;
                        updateVM.Task.LastUpdatedAt = DateTime.Now;
                        updateVM.Task.LastUpdatedBy = currentUserName;
                        updateVM.Task.TaskProgress = taskProgress;
                        updateVM.Task.TaskStatus = _context.TaskStatus.Where(x => x.Id == updateVM.SelectedStatus).SingleOrDefault();
                        _context.Update(updateVM.Task);
                        
                        TaskFollowUpResponse taskFollowUpResponse = new TaskFollowUpResponse();
                        taskFollowUpResponse.RespondedBy= currentUserName;
                        taskFollowUpResponse.LastUpdatedAt = DateTime.Now;
                        taskFollowUpResponse.CreatedAt = DateTime.Now;
                        taskFollowUpResponse.TaskResponse = taskProgress;
                        taskFollowUpResponse.Task = updateVM.Task;
                        _context.Add(taskFollowUpResponse);

                        TaskAudit followUpResponseAudit = new TaskAudit();
                        followUpResponseAudit.ActionDate = DateTime.Now;
                        followUpResponseAudit.ActionBy = currentUserName;
                        followUpResponseAudit.Description = "Follow up taken";
                        followUpResponseAudit.Task = thisTask;
                        followUpResponseAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.FollowUpResponse).SingleOrDefault();
                        _context.Add(followUpResponseAudit);

                        if (previousProgress != taskProgress)
                        {
                            //Audit this update task..
                            TaskAudit progressAudit = new TaskAudit();
                            progressAudit.ActionDate = DateTime.Now;
                            progressAudit.ActionBy = currentUserName;
                            progressAudit.Description = "Progress updated to " + taskProgress;
                            progressAudit.Task = thisTask;
                            progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Assignment).SingleOrDefault();
                            _context.Add(progressAudit);
                        }

                        await _context.SaveChangesAsync();
                        List<string> employeeList = GetTaskEmployeeEmailAddresses(id, currentUserName);
                        await MailSystem.SendEmail(MailSystem.FollowUpResponseEmailTemplate, employeeList);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(updateVM.Task.Id))
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
            return View(updateVM);
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

        [HttpPost, ActionName("DeleteTask")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            bool isDeleted = false;
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                task.IsDeleted = true;
                _context.Tasks.Attach(task);
                _context.Entry(task).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                isDeleted = true;
            }
            catch(Exception ex)
            {
                
            }

            return new JsonResult(new { isDeleted = isDeleted });
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

        /// <summary>
        /// Gets the task history of a particular Task
        /// </summary>
        /// <returns>Task history View Model</returns>
        public async Task<IActionResult> TaskHistory()
        {
            TaskHistoryViewModel taskHistoryVM = new TaskHistoryViewModel();
            taskHistoryVM.AssignmentHistory = new List<TaskAudit>();
            taskHistoryVM.FollowUpHistory = new List<TaskAudit>();
            taskHistoryVM.FollowUpResponseHistory = new List<TaskAudit>();
            taskHistoryVM.ProgressHistory = new List<TaskAudit>();
            taskHistoryVM.SubTasksHistory = new List<TaskAudit>();

            List<TaskAudit> taskAuditList = await _context.TaskAudit.Where(x => x.Id == 4).ToListAsync();
            taskHistoryVM.ProgressHistory = taskAuditList.Where(x => x.Type.Id == (int)Common.Common.AuditType.Progress).ToList();
            taskHistoryVM.FollowUpHistory = taskAuditList.Where(x => x.Type.Id == (int)Common.Common.AuditType.FollowUp).ToList();
            taskHistoryVM.FollowUpResponseHistory = taskAuditList.Where(x => x.Type.Id == (int)Common.Common.AuditType.FollowUpResponse).ToList();
            taskHistoryVM.AssignmentHistory = taskAuditList.Where(x => x.Type.Id == (int)Common.Common.AuditType.Assignment).ToList();
            taskHistoryVM.SubTasksHistory = taskAuditList.Where(x => x.Type.Id == (int)Common.Common.AuditType.SubTasks).ToList();

            return View(taskHistoryVM);
        }


    }
}
