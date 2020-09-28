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
        public string currentUserName;

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
        public IActionResult Index()
        {
            if (TempData.ContainsKey("deletionId"))
            {
                TaskManager.Data.Task deletedTask = _context.Tasks.Where(x => x.Id == Convert.ToInt32(TempData["deletionId"])).SingleOrDefault();
                ViewData["deletionMessage"] = "Task with ID => " + deletedTask.Id + " has been deleted";
            }
            TaskViewModel taskVM = new TaskViewModel();
            taskVM.CurrentUserName = currentUserName;
            ViewData["UserName"] = currentUserName;
            //return View(await _context.Tasks.Where(x => x.IsDeleted != true).ToListAsync());
            return View(taskVM);
        }

        public IActionResult LoadTasksData()
        {
            try
            {
                var tasks = _context.Tasks
                            .Include(x => x.TaskStatus)
                            .Where(x => x.IsDeleted != true).OrderByDescending(x => x.LastUpdatedAt).ToList();
                var taskEmployees = (from c in _context.TaskEmployees
                                     join o in _context.Employees
                                     on c.UserName equals o.UserName
                                     join tc in _context.TaskCapacities
                                     on c.TaskCapacity.Id equals tc.Id
                                     join dept in _context.Departments
                                     on o.Department.Id equals dept.Id
                                     select new
                                     {
                                         UserName = o.UserName,
                                         EmployeeName = o.EmployeeName,
                                         TaskId = c.Task.Id,
                                         CapacityId = tc.Id,
                                         DeptId = o.Department.Id,
                                         ManagerUserName = o.Department.Manager.UserName
                                     }).ToList();

                List<Department> departments = _context.Departments
                    .Include(x => x.ParentDepartment)
                    .Include(x => x.Manager).OrderBy(x => x.Id).ToList();

                //total number of rows count 
                int recordsTotal = tasks.Count;
                List<TasksGridViewModel> tasksGridVMList = new List<TasksGridViewModel>();
                foreach (TaskManager.Data.Task task in tasks)
                {
                    if (taskEmployees.Where(x => x.TaskId == task.Id && x.UserName == currentUserName).Any()
                        || taskEmployees.Where(x => x.TaskId == task.Id && x.ManagerUserName == currentUserName).Any()
                        || IsThisUserManagingThisDepartment(taskEmployees.Where(x => x.TaskId == task.Id).FirstOrDefault().DeptId,
                            departments, currentUserName))
                    {
                        TasksGridViewModel taskGridVM = new TasksGridViewModel();

                        taskGridVM.TaskId = task.Id;
                        taskGridVM.Progress = task.TaskProgress;
                        taskGridVM.TargetDate = task.Target.ToString("dd-MMM-yyyy HH:mm:ss");
                        taskGridVM.LastUpdated = task.LastUpdatedAt.ToString("dd-MMM-yyyy HH:mm:ss");
                        taskGridVM.Status = task.TaskStatus.Status;
                        taskGridVM.Title = task.Title;

                        string assigneeUserName = taskEmployees.Where(x => x.CapacityId == (int)Common.Common.TaskCapacity.Assignee
                            && x.TaskId == task.Id).FirstOrDefault().UserName;

                        string creatorUserName = taskEmployees.Where(x => x.CapacityId == (int)Common.Common.TaskCapacity.Creator
                            && x.TaskId == task.Id).FirstOrDefault().UserName;

                        taskGridVM.CreatedBy = creatorUserName;
                        taskGridVM.AssignedTo = assigneeUserName;
                        tasksGridVMList.Add(taskGridVM);
                    }
                }
                //Returning Json Data
                return Json(new { draw = "1", recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = tasksGridVMList });

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public bool IsThisUserManagingThisDepartment(int thisDepartmentId, List<Department> departments, string userName)
        {
            bool isThisUserManager = false;
            Department thisDepartment = departments.Where(x => x.Id == thisDepartmentId).SingleOrDefault().ParentDepartment;
            while (thisDepartment != null)
            {
                if (thisDepartment.Manager.UserName == userName)
                {
                    isThisUserManager = true;
                    break;
                }
                thisDepartment = thisDepartment.ParentDepartment;
            }
            return isThisUserManager;
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
            List<Employee> employees = _context.Employees.ToList();
            popuLateTaskViewDropDowns(taskVM, employees);
            return PartialView("_Create", taskVM);
        }

        public ActionResult CreateMultiple()
        {
            TaskMultipleCreateViewModel taskVM = new TaskMultipleCreateViewModel();
            return PartialView("_CreateMultiple", taskVM);
        }

        private void popuLateTaskViewDropDowns(TaskCreateViewModel taskVm, List<Employee> employeeList)
        {
            List<SelectListItem> employeeDrpDwnList = new List<SelectListItem>();
            List<SelectListItem> taskPriorityDrpDwnList = new List<SelectListItem>();
            var taskPriorityEnumsList = Enum.GetValues(typeof(Common.Common.TaskPriority));
            foreach (Common.Common.TaskPriority priority in taskPriorityEnumsList)
            {
                taskPriorityDrpDwnList.Add(new SelectListItem() { Text = priority.ToString(), Value = ((int)priority).ToString() });
            }
            taskVm.PriorityList = taskPriorityDrpDwnList;

            foreach (Employee employee in employeeList)
            {
                //if (employee.UserName != currentUserName)
                //{
                    employeeDrpDwnList.Add(new SelectListItem() { Text = employee.EmployeeName, Value = employee.UserName.ToString() });
                //}
            }
            taskVm.EmployeeList = employeeDrpDwnList;
            taskVm.Target = DateTime.Now;
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
                    taskAssignee.IsActive = true;
                    _context.Add(taskAssignee);

                    //Add task creator
                    TaskEmployee taskCreator = new TaskEmployee();
                    taskCreator.Task = task;
                    int taskCreatorCapacity = (int)TaskManager.Common.Common.TaskCapacity.Creator;
                    taskCreator.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskCreatorCapacity).SingleOrDefault();
                    taskCreator.UserName = currentUserName;
                    taskCreator.IsActive = true;
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

                    List<Employee> employeeList = _context.Employees.ToList();
                    popuLateTaskViewDropDowns(taskCreateVM, employeeList);
                }
                else
                {
                    List<Employee> employeeList = _context.Employees.ToList();
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
                .Where(x => x.Task.Id == id && x.IsActive == true)
                .Select(x => x.UserName).Distinct()
                .ToListAsync();
            foreach (string employeeUserName in taskEmployees)
            {
                updateProgressVM.TaskEmployees.Add(employeeUserName);
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
                            progressAudit.Description = "Progress updated to " + taskProgress + "%";
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

        private void popuLateTaskEditViewDropDowns(TaskViewModel taskVm, List<Employee> employeeList)
        {
            List<SelectListItem> employeeDrpDwnList = new List<SelectListItem>();
            List<SelectListItem> taskPriorityDrpDwnList = new List<SelectListItem>();
            var taskPriorityEnumsList = Enum.GetValues(typeof(Common.Common.TaskPriority));
            foreach (Common.Common.TaskPriority priority in taskPriorityEnumsList)
            {
                taskPriorityDrpDwnList.Add(new SelectListItem() { Text = priority.ToString(), Value = ((int)priority).ToString() });
            }
            taskVm.PriorityList = taskPriorityDrpDwnList;

            foreach (Employee employee in employeeList)
            {
                if (employee.UserName != currentUserName)
                {
                    employeeDrpDwnList.Add(new SelectListItem() { Text = employee.EmployeeName, Value = employee.UserName.ToString() });
                }
            }
            taskVm.EmployeeList = employeeDrpDwnList;
        }

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
            popuLateTaskEditViewDropDowns(taskVM, _context.Employees.ToList());
            taskVM.PriorityId = task.TaskPriority.Id;
            TaskEmployee currentEmployee = _context.TaskEmployees.Where(x => x.Task.Id == id
                && x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee).FirstOrDefault();

            taskVM.AssigneeCode = currentEmployee.UserName;
            taskVM.CurrentUserName = currentUserName;
            ViewData["UserName"] = currentUserName;
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


        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }


    }
}
