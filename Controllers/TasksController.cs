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
        public TasksController(TaskManagerContext context, IConfiguration configuration) : base(context, configuration)
        {
            currentUserName = configuration.GetSection("TaskManagerUserName").Value;
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
            SetUserName();
            if (TempData.ContainsKey("deletionId"))
            {
                TaskManager.Data.Task deletedTask = _context.Tasks.Where(x => x.Id == Convert.ToInt32(TempData["deletionId"])).SingleOrDefault();
                ViewData["deletionMessage"] = "Task with ID => " + deletedTask.Id + " has been deleted";
            }
            TaskViewModel taskVM = new TaskViewModel();
            taskVM.CurrentUserName = currentUserName;
            ViewData["UserName"] = currentUserName;
            Employee employee = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
            ViewData["EmployeeName"] = employee == null ? "" : employee.EmployeeName;
            //return View(await _context.Tasks.Where(x => x.IsDeleted != true).ToListAsync());
            return View(taskVM);
        }

        protected bool IsThisUserManagerOfThisDepartment(Department thisDepartment, string userName)
        {
            bool isThisUserManager = false;

            string managerUserName = _context.Departments.Where(x => x.Id == thisDepartment.Id)
                .Select(x => x.Manager).SingleOrDefault().UserCode;

            if (managerUserName == userName)
            {
                return true;
            }
            Department parentDepartment = thisDepartment.ParentDepartment;
            if (parentDepartment != null)
            {
                thisDepartment = parentDepartment;
                while (thisDepartment != null)
                {
                    managerUserName = _context.Departments.Where(x => x.Id == thisDepartment.Id).Select(x => x.Manager).SingleOrDefault().UserCode;
                    if (managerUserName == userName)
                    {
                        isThisUserManager = true;
                        break;
                    }
                    thisDepartment = thisDepartment.ParentDepartment;
                }
            }
            else
            {
                managerUserName = _context.Departments.Where(x => x.Id == thisDepartment.Id).Select(x => x.Manager).SingleOrDefault().UserCode;
                isThisUserManager = managerUserName == userName;
            }
            return isThisUserManager;
        }

        protected bool IsThisUserManagerOfThisDepartment(List<Department> alldepartments, List<int> selectedDepartmentIds, string userName)
        {
            bool isThisUserManager = false;
            try
            {
                List<Department> selectedDepts = alldepartments.Where(x => selectedDepartmentIds.Any(y => y == x.Id)).ToList();

                foreach (Department department in selectedDepts)
                {
                    isThisUserManager = IsThisUserManagerOfThisDepartment(department, userName);
                    if (isThisUserManager)
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return isThisUserManager;
        }

        public IActionResult LoadTasksData(string username)
        {
            try
            {
                if (!String.IsNullOrEmpty(username))
                {
                    currentUserName = username;
                }
                var tasks = _context.Tasks
                            .Include(x => x.TaskStatus)
                            .Where(x => x.IsDeleted != true).OrderByDescending(x => x.LastUpdatedAt).ToList();
                var taskEmployees = (from c in _context.TaskEmployees
                                     .Where(k => k.IsActive == true && k.Task.IsDeleted == false)
                                     join o in _context.Employees
                                     on c.Employee.UserCode equals o.UserCode
                                     join tc in _context.TaskCapacities
                                     on c.TaskCapacity.Id equals tc.Id
                                     join dept in _context.Departments
                                     on o.Department.Id equals dept.Id
                                     select new
                                     {
                                         UserName = o.UserCode,
                                         EmployeeName = o.EmployeeName,
                                         TaskId = c.Task.Id,
                                         CapacityId = tc.Id,
                                         DeptId = o.Department.Id,
                                         ManagerUserName = o.Department.Manager.UserCode
                                     }).ToList();

                List<Department> departments = _context.Departments
                    .Include(x => x.ParentDepartment)
                    .Include(x => x.Manager).OrderBy(x => x.Id)
                    .ToList();

                //total number of rows count 
                int recordsTotal = tasks.Count;
                List<TasksGridViewModel> tasksGridVMList = new List<TasksGridViewModel>();
                foreach (TaskManager.Data.Task task in tasks)
                {
                    if (taskEmployees.Where(x => x.TaskId == task.Id && x.UserName == currentUserName).Any()
                        || taskEmployees.Where(x => x.TaskId == task.Id 
                        && IsThisUserManagerOfThisDepartment(
                            departments,
                            taskEmployees.Where(x => x.TaskId == task.Id).Select(x => x.DeptId).ToList(),
                           currentUserName)).Any()
                        )
                    {
                        TasksGridViewModel taskGridVM = new TasksGridViewModel();

                        taskGridVM.TaskId = task.Id;
                        taskGridVM.Progress = task.TaskProgress;
                        taskGridVM.TargetDate = task.Target.ToString("dd-MMM-yyyy HH:mm:ss");
                        taskGridVM.LastUpdated = task.LastUpdatedAt.ToString("dd-MMM-yyyy HH:mm:ss");
                        taskGridVM.Status = task.TaskStatus.Status;
                        taskGridVM.Title = task.Title;

                        var assignee = taskEmployees.Where(x => x.CapacityId == (int)Common.Common.TaskCapacity.Assignee
                            && x.TaskId == task.Id).FirstOrDefault();

                        string creatorUserName = taskEmployees.Where(x => x.CapacityId == (int)Common.Common.TaskCapacity.Creator
                            && x.TaskId == task.Id).FirstOrDefault().UserName;

                        taskGridVM.IsEditable = taskEmployees.Where(x => x.TaskId == task.Id && x.UserName == currentUserName
                        && (x.CapacityId == (int)Common.Common.TaskCapacity.Creator
                         || x.CapacityId == (int)Common.Common.TaskCapacity.Assignee
                         || x.CapacityId == (int)Common.Common.TaskCapacity.SubTaskAssignee
                         || x.CapacityId == (int)Common.Common.TaskCapacity.Follower
                         )
                        ).Any();
                        taskGridVM.CreatedBy = creatorUserName;
                        taskGridVM.AssignedTo = assignee.UserName;
                        taskGridVM.AssignedToEmployeeName = assignee.EmployeeName;
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

        private AssigneeViewModel PopulateAllAssignee(string userName)
        {
            AssigneeViewModel assigneeVM = new AssigneeViewModel();
            List<Employee> employeeList = _context.Employees.ToList();
            assigneeVM.Assignees = new List<SelectListItem>();

            foreach (Employee employee in employeeList)
            {
                assigneeVM.Assignees.Add(new SelectListItem()
                {
                    Text = employee.UserCode,
                    Value = employee.UserCode,
                    Selected = userName == employee.UserCode ? true : false
                });
            }
            return assigneeVM;
        }

        public IActionResult AllAssignees(string userName)
        {
            AssigneeViewModel assigneeVM = PopulateAllAssignee(userName);
            return PartialView("_Assignee", assigneeVM);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateAssignee(AssigneeViewModel assigneeVM)
        {
            Data.Task thisTask = _context.Tasks.Where(x => x.Id == assigneeVM.AssigneeTaskId).SingleOrDefault();
            if (thisTask != null)
            {
                TaskEmployee employee = _context.TaskEmployees.Where(x =>
                   x.Task.Id == assigneeVM.AssigneeTaskId
                   && x.TaskCapacity.Id == (int)TaskManager.Common.Common.TaskCapacity.Assignee
                   ).FirstOrDefault();
                if (employee != null)
                {
                    if (employee.Employee.UserCode != assigneeVM.SelectedAssignee)
                    {
                        Employee currentEmployee = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
                        Employee selectedAssignee = _context.Employees.Where(x => x.UserCode == assigneeVM.SelectedAssignee).SingleOrDefault();
                        employee.Employee = selectedAssignee;
                        employee.IsActive = true;
                        _context.TaskEmployees.Attach(employee);
                        _context.Entry(employee).State = EntityState.Modified;

                        TaskAudit assigneeAudit = new TaskAudit();
                        assigneeAudit.ActionDate = DateTime.Now;
                        assigneeAudit.ActionBy = currentEmployee;
                        assigneeAudit.Description = "Task " + thisTask.Title + " assignment updated. Assigned to " + selectedAssignee.EmployeeName;
                        assigneeAudit.Task = thisTask;
                        assigneeAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Assignment).SingleOrDefault();
                        _context.Add(assigneeAudit);

                        thisTask.LastUpdatedBy = currentEmployee;
                        thisTask.LastUpdatedAt = DateTime.Now;

                        _context.Tasks.Attach(thisTask);
                        _context.Entry(thisTask).State = EntityState.Modified;

                        await _context.SaveChangesAsync();
                    }
                }
            }

            assigneeVM = PopulateAllAssignee(assigneeVM.SelectedAssignee);
            return PartialView("_Assignee", assigneeVM);
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

        public ActionResult Create(string userName)
        {
            TaskCreateViewModel taskVM = new TaskCreateViewModel();
            List<Employee> employees = _context.Employees.ToList();
            popuLateTaskViewDropDowns(taskVM, userName);
            taskVM.TargetVal = DateTime.Now;
            return PartialView("_Create", taskVM);
        }

        public async Task<ActionResult> EditTask(int taskId)
        {
            TaskUpdateViewModel taskVM = new TaskUpdateViewModel();
            List<Employee> employees = _context.Employees.ToList();
            SetUserName();

            var task = await _context.Tasks
               .Include(x => x.TaskPriority)
               .Include(x => x.TaskStatus)
               .Where(x => x.Id == taskId).SingleOrDefaultAsync();

            taskVM.PriorityId = task.TaskPriority.Id;
            TaskEmployee currentEmployee = _context.TaskEmployees
                .Include(x => x.Employee)
                .Where(x => x.Task.Id == taskId
                && x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee).FirstOrDefault();
            popuLateTaskViewDropDowns(taskVM, currentUserName);
            taskVM.Description = task.Description;
            taskVM.Title = task.Title;
            taskVM.Target = task.Target;
            taskVM.TargetVal = task.Target;
            taskVM.AssigneeCode = currentEmployee.Employee.UserCode;

            return PartialView("_Edit", taskVM);
        }

        public ActionResult CreateMultiple()
        {
            TaskMultipleCreateViewModel taskVM = new TaskMultipleCreateViewModel();
            return PartialView("_CreateMultiple", taskVM);
        }

        private void popuLateTaskViewDropDowns(TaskCreateViewModel taskVm, string userName)
        {
            List<SelectListItem> taskPriorityDrpDwnList = new List<SelectListItem>();
            var taskPriorityEnumsList = Enum.GetValues(typeof(Common.Common.TaskPriority));
            foreach (Common.Common.TaskPriority priority in taskPriorityEnumsList)
            {
                taskPriorityDrpDwnList.Add(new SelectListItem() { Text = priority.ToString(), Value = ((int)priority).ToString() });
            }
            taskVm.PriorityList = taskPriorityDrpDwnList;

            taskVm.EmployeeList = this.LoadAssigneeDrpDwnData(userName);
            taskVm.Target = DateTime.Now;
        }

        private void popuLateTaskViewDropDowns(TaskUpdateViewModel taskVm, string userName)
        {
            List<SelectListItem> taskPriorityDrpDwnList = new List<SelectListItem>();
            var taskPriorityEnumsList = Enum.GetValues(typeof(Common.Common.TaskPriority));
            foreach (Common.Common.TaskPriority priority in taskPriorityEnumsList)
            {
                taskPriorityDrpDwnList.Add(new SelectListItem() { Text = priority.ToString(), Value = ((int)priority).ToString() });
            }
            taskVm.PriorityList = taskPriorityDrpDwnList;

            taskVm.EmployeeList = this.LoadAssigneeDrpDwnData(userName);
            taskVm.Target = DateTime.Now;
        }


        [HttpPost]
        public ActionResult CreateMultipleTasks(TaskCreateViewModel taskCreateVM)
        {
            SetUserName();
            if (ModelState.IsValid)
            {
            }
            List<Employee> employeeList = _context.Employees.ToList();
            popuLateTaskViewDropDowns(taskCreateVM, currentUserName);
            taskCreateVM.Title = string.Empty;

            return PartialView("_Create", taskCreateVM);
        }


        [HttpPost]
        public async Task<ActionResult> Create(TaskCreateViewModel taskCreateVM)
        {
            try
            {
                SetUserName();
                if (ModelState.IsValid)
                {
                    Employee assignee = _context.Employees.Where(x => x.UserCode == taskCreateVM.AssigneeCode).SingleOrDefault();
                    Employee currentUser = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();

                    Data.Task task = new Data.Task();
                    task.IsDeleted = false;
                    task.LastUpdatedAt = DateTime.Now;
                    task.LastUpdatedBy = currentUser;
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
                    taskAssignee.Employee = assignee;
                    taskAssignee.IsActive = true;
                    _context.Add(taskAssignee);

                    //Add task creator
                    TaskEmployee taskCreator = new TaskEmployee();
                    taskCreator.Task = task;
                    int taskCreatorCapacity = (int)TaskManager.Common.Common.TaskCapacity.Creator;
                    taskCreator.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskCreatorCapacity).SingleOrDefault();
                    taskCreator.Employee = currentUser;
                    taskCreator.IsActive = true;
                    _context.Add(taskCreator);

                    //Audit this task creation..
                    TaskAudit progressAudit = new TaskAudit();
                    progressAudit.ActionDate = DateTime.Now;
                    string description = taskCreateVM.Description == null ? "" : "Description : " + taskCreateVM.Description;
                    description += " , Target Date : " + taskCreateVM.Target.ToString("dd-MMM-yyyy");
                    description += " , Priority : " + task.TaskPriority.Priority;
                    progressAudit.ActionBy = _context.Employees.Where(x =>x.UserCode == currentUserName).SingleOrDefault();
                    progressAudit.Description = "Task with title " + task.Title + " created. Details are : " + description;
                    progressAudit.Task = task;
                    progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Progress).SingleOrDefault();
                    _context.Add(progressAudit);

                    TaskAudit assignmentAudit = new TaskAudit();
                    assignmentAudit.ActionDate = DateTime.Now;
                    assignmentAudit.ActionBy = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
                    assignmentAudit.Description = "Task with title " + task.Title + " created. Assigned to " + _context.Employees.Where(x => x.UserCode == taskCreateVM.AssigneeCode).SingleOrDefault().EmployeeName;
                    assignmentAudit.Task = task;
                    assignmentAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Assignment).SingleOrDefault();
                    _context.Add(assignmentAudit);
                    await _context.SaveChangesAsync();

                    List<Employee> employeeList = _context.Employees.ToList();
                    popuLateTaskViewDropDowns(taskCreateVM, currentUserName);
                }
                else
                {
                    List<Employee> employeeList = _context.Employees.ToList();
                    popuLateTaskViewDropDowns(taskCreateVM, currentUserName);
                }
            }
            catch (Exception ex)
            {
                
            }

            return PartialView("_Create", taskCreateVM);
        }


        [HttpPost]
        public async Task<ActionResult> EditTask(TaskUpdateViewModel taskCreateVM)
        {
            try
            {
                SetUserName();
                if (ModelState.IsValid)
                {
                    Data.Task task = _context.Tasks
                        .Include(x =>x.TaskPriority)
                        .Where(x => x.Id == taskCreateVM.Id).SingleOrDefault();
                    Employee currentEmploye = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();

                    task.LastUpdatedAt = DateTime.Now;
                    task.LastUpdatedBy = currentEmploye;
                    var employees = _context.Employees.ToList();

                    string previousTitle = task.Title;
                    if (taskCreateVM.Title != previousTitle)
                    {
                        task.Title = taskCreateVM.Title;
                        TaskAudit titleAudit = new TaskAudit();
                        titleAudit.ActionDate = DateTime.Now;
                        titleAudit.ActionBy = currentEmploye;
                        titleAudit.Description = "Task updated. Title changed from " + previousTitle + " to " + taskCreateVM.Title;
                        titleAudit.Task = task;
                        titleAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Update).SingleOrDefault();
                        _context.Add(titleAudit);
                    }
                    DateTime previousTarget = task.Target;
                    if (taskCreateVM.Target.Date != previousTarget.Date)
                    {
                        task.Target = taskCreateVM.Target;
                        TaskAudit targetAudit = new TaskAudit();
                        targetAudit.ActionDate = DateTime.Now;
                        targetAudit.ActionBy = currentEmploye;
                        targetAudit.Description = "Task with title " + task.Title +" Updated. Target changed from " + previousTarget.Date.ToString("dd-MMM-yyyy") + " to " + taskCreateVM.Target.Date.ToString("dd-MMM-yyyy");
                        targetAudit.Task = task;
                        targetAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Update).SingleOrDefault();
                        _context.Add(targetAudit);
                    }
                    string previousDescription = task.Description;
                    if (taskCreateVM.Description != previousDescription)
                    {
                        task.Description = taskCreateVM.Description;
                        TaskAudit descriptionAudit = new TaskAudit();
                        descriptionAudit.ActionDate = DateTime.Now;
                        descriptionAudit.ActionBy = currentEmploye;
                        descriptionAudit.Description = "Task with title " + task.Title + " Updated. Description changed from " + previousDescription + " to " + taskCreateVM.Description;
                        descriptionAudit.Task = task;
                        descriptionAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Update).SingleOrDefault();
                        _context.Add(descriptionAudit);
                    }
                    TaskPriority modifiedPriority = _context.TaskPriority.Where(x => x.Id == taskCreateVM.PriorityId).SingleOrDefault();
                    if (task.TaskPriority.Id != modifiedPriority.Id)
                    {
                        task.Description = taskCreateVM.Description;
                        TaskAudit descriptionAudit = new TaskAudit();
                        descriptionAudit.ActionDate = DateTime.Now;
                        descriptionAudit.ActionBy = currentEmploye;
                        descriptionAudit.Description = "Task with title " + task.Title + " Updated. Priority changed from " + task.TaskPriority.Priority + " to " + modifiedPriority.Priority;
                        task.TaskPriority = modifiedPriority;
                        descriptionAudit.Task = task;
                        descriptionAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Update).SingleOrDefault();
                        _context.Add(descriptionAudit);
                    }

                   TaskEmployee employee = _context.TaskEmployees
                        .Include(x => x.Employee)
                        .Where(x =>
                       x.Task.Id == taskCreateVM.Id
                       && x.TaskCapacity.Id == (int)TaskManager.Common.Common.TaskCapacity.Assignee
                       ).FirstOrDefault();
                    if (employee != null)
                    {
                        if (employee.Employee.UserCode != taskCreateVM.AssigneeCode)
                        {
                            string previousEmployeeName = employees.Where(x => x.UserCode == employee.Employee.UserCode).SingleOrDefault().EmployeeName;
                            Employee assignedEmployee = employees.Where(x => x.UserCode == taskCreateVM.AssigneeCode).SingleOrDefault();
                            employee.Employee = assignedEmployee;
                            employee.IsActive = true;
                            _context.TaskEmployees.Attach(employee);
                            _context.Entry(employee).State = EntityState.Modified;

                            TaskAudit assigneeAudit = new TaskAudit();
                            assigneeAudit.ActionDate = DateTime.Now;
                            assigneeAudit.ActionBy = currentEmploye;
                            string modifiedEmployeeName = assignedEmployee.EmployeeName;
                            assigneeAudit.Description = "Task with title " + task.Title + " Updated. Assignment changed from " + previousEmployeeName + " to " + modifiedEmployeeName;
                            assigneeAudit.Task = task;
                            assigneeAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Assignment).SingleOrDefault();
                            _context.Add(assigneeAudit);
                        }
                    }

                    _context.Tasks.Attach(task);
                    _context.Entry(task).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    popuLateTaskViewDropDowns(taskCreateVM, currentUserName);
                }
                else
                {
                    popuLateTaskViewDropDowns(taskCreateVM, currentUserName);
                }
            }
            catch (Exception ex)
            {

            }

            return PartialView("_Edit", taskCreateVM);
        }



        [HttpPost]
        public async Task<ActionResult> CreateMultiple(TaskMultipleCreateViewModel taskCreateVM)
        {
            try
            {
                SetUserName();
                if (ModelState.IsValid)
                {
                    string[] listOfTaskTitles = taskCreateVM.ListOfTaskTitles.Split(",");
                    foreach (string title in listOfTaskTitles)
                    {
                        Employee currentEmployee = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
                        Employee assingnedEmployee = _context.Employees.Where(x => x.UserCode == taskCreateVM.AssigneeCode).SingleOrDefault();
                        Data.Task task = new Data.Task();
                        task.IsDeleted = false;
                        task.LastUpdatedAt = DateTime.Now;
                        task.LastUpdatedBy = currentEmployee;
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
                        taskAssignee.Employee = assingnedEmployee;
                        _context.Add(taskAssignee);

                        //Add task creator
                        TaskEmployee taskCreator = new TaskEmployee();
                        taskCreator.Task = task;
                        int taskCreatorCapacity = (int)TaskManager.Common.Common.TaskCapacity.Creator;
                        taskCreator.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskCreatorCapacity).SingleOrDefault();
                        taskCreator.Employee = currentEmployee;
                        _context.Add(taskCreator);

                        //Audit this task creation..
                        TaskAudit progressAudit = new TaskAudit();
                        progressAudit.ActionDate = DateTime.Now;
                        progressAudit.ActionBy = currentEmployee;
                        progressAudit.Description = "Task with title " + task.Title + " created. Progess is 0%";
                        progressAudit.Task = task;
                        progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Progress).SingleOrDefault();
                        _context.Add(progressAudit);

                        TaskAudit assignmentAudit = new TaskAudit();
                        assignmentAudit.ActionDate = DateTime.Now;
                        assignmentAudit.ActionBy = currentEmployee;
                        assignmentAudit.Description = "Task with title " + task.Title + " created. Assigned to " + assingnedEmployee.EmployeeName;
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
            var taskStatusEnumsList = Enum.GetValues(typeof(Common.Common.TaskStatus));
            foreach (Common.Common.TaskStatus taskStatus in taskStatusEnumsList)
            {
                if (taskStatus != Common.Common.TaskStatus.AllTasks) {
                    statusDrpDwnList.Add(new SelectListItem() { Text = Common.Common.GetTaskStatusDescription((int)taskStatus), Value = ((int)taskStatus).ToString() });
                }
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
            updateProgressVM.Remarks = task.ProgressRemarks;
            updateProgressVM.Status = task.TaskStatus.Status;
            updateProgressVM.TaskProgress = task.TaskProgress;
            populateUpdateProgressVMDropDowns(updateProgressVM);
            updateProgressVM.TaskEmployees = new List<string>();
            var taskEmployees = await _context.TaskEmployees
                .Where(x => x.Task.Id == id && x.IsActive == true)
                .Select(x => x.Employee.UserCode).Distinct()
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
            SetUserName();
            if (ModelState.IsValid)
            {
                try
                {
                    TaskEmployee currentEmployee = _context.TaskEmployees
                        .Include(x => x.TaskCapacity)
                        .Where(x => x.Employee.UserCode == currentUserName && x.Task.Id == id).FirstOrDefault();
                    TaskManager.Data.Task thisTask = _context.Tasks
                        .Include(X => X.TaskStatus)
                        .Where(x => x.Id == id).SingleOrDefault();

                    Employee currentEmployeeObj = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();

                    string previousProgress = thisTask.TaskProgress;
                    string taskProgress = updateVM.TaskProgress;
                    string previousRemarks = thisTask.ProgressRemarks;
                    int previousStatusId = thisTask.TaskStatus.Id;
                    thisTask.LastUpdatedAt = DateTime.Now;
                    thisTask.LastUpdatedBy = currentEmployeeObj;
                    thisTask.TaskProgress = taskProgress;
                    thisTask.ProgressRemarks = updateVM.Remarks;
                    thisTask.TaskStatus = _context.TaskStatus.Where(x => x.Id == Convert.ToInt32(updateVM.Status)).SingleOrDefault();
                    _context.Update(thisTask);

                    TaskFollowUp followUp = _context.TaskFollowUps.Where(x => x.Task.Id == id).FirstOrDefault();
                    if (followUp != null)
                    {
                        TaskFollowUpResponse taskFollowUpResponse = new TaskFollowUpResponse();
                        taskFollowUpResponse.RespondedBy = currentEmployeeObj;
                        taskFollowUpResponse.LastUpdatedAt = DateTime.Now;
                        taskFollowUpResponse.CreatedAt = DateTime.Now;
                        taskFollowUpResponse.TaskResponse = taskProgress;
                        taskFollowUpResponse.Task = thisTask;
                        _context.Add(taskFollowUpResponse);

                        TaskAudit followUpResponseAudit = new TaskAudit();
                        followUpResponseAudit.ActionDate = DateTime.Now;
                        followUpResponseAudit.ActionBy = currentEmployeeObj;
                        followUpResponseAudit.Description = "Follow up response provided with progress update from " + previousProgress + "% to " + taskProgress + "%";
                        followUpResponseAudit.Description += " and remarks " + updateVM.Remarks;
                        followUpResponseAudit.Task = thisTask;
                        followUpResponseAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.FollowUpResponse).SingleOrDefault();
                        _context.Add(followUpResponseAudit);
                    }

                    if (previousProgress != taskProgress)
                    {
                        TaskAudit progressAudit = new TaskAudit();
                        progressAudit.ActionDate = DateTime.Now;
                        progressAudit.ActionBy = currentEmployeeObj;
                        progressAudit.Description = "Task with title " + thisTask.Title + " progress updated from " + previousProgress + "% to " + taskProgress + "%";
                        progressAudit.Task = thisTask;
                        progressAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Progress).SingleOrDefault();
                        _context.Add(progressAudit);
                    }

                    if (previousRemarks != updateVM.Remarks)
                    {
                        TaskAudit remarksAudit = new TaskAudit();
                        remarksAudit.ActionDate = DateTime.Now;
                        remarksAudit.ActionBy = currentEmployeeObj;
                        string remarksDescription = string.Empty;
                        if (previousRemarks != null)
                        {
                            remarksDescription = "Task with title " + thisTask.Title + " updated. Remarks updated from " + previousRemarks + " to " + updateVM.Remarks;
                        }
                        else
                        {
                            remarksDescription = "Task with title " + thisTask.Title + " updated. Remarks are " + updateVM.Remarks;
                        }
                        remarksAudit.Description = remarksDescription;
                        remarksAudit.Task = thisTask;
                        remarksAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Update).SingleOrDefault();
                        _context.Add(remarksAudit);
                    }
                    
                    if (previousStatusId != thisTask.TaskStatus.Id)
                    {
                        TaskAudit statusAudit = new TaskAudit();
                        statusAudit.ActionDate = DateTime.Now;
                        statusAudit.ActionBy = currentEmployeeObj;
                        statusAudit.Description = "Task with title " + thisTask.Title + " updated. Status updated from " + Common.Common.GetTaskStatusDescription(previousStatusId) + " to " + Common.Common.GetTaskStatusDescription(thisTask.TaskStatus.Id);
                        statusAudit.Task = thisTask;
                        statusAudit.Type = _context.AuditType.Where(x => x.Id == (int)Common.Common.AuditType.Update).SingleOrDefault();
                        _context.Add(statusAudit);
                    }


                    await _context.SaveChangesAsync();
                    //List<string> employeeList = GetTaskEmployeeEmailAddresses(id, currentUserName);
                    //await MailSystem.SendEmail(MailSystem.FollowUpResponseEmailTemplate, employeeList);
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
                .Include(X => X.Employee)
                .Where(x => x.Task.Id == id).ToListAsync();
            foreach (TaskEmployee employee in taskEmployees)
            {
                updateVM.TaskEmployees.Add(employee.Employee.EmployeeName);
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
                if (employee.UserCode != currentUserName)
                {
                    employeeDrpDwnList.Add(new SelectListItem() { Text = employee.EmployeeName, Value = employee.UserCode.ToString() });
                }
            }
            taskVm.EmployeeList = employeeDrpDwnList;
        }

        public async Task<IActionResult> Edit(int? id, string username, int viewMode)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!String.IsNullOrEmpty(username))
            {
                currentUserName = username;
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
            TaskEmployee currentEmployee = _context.TaskEmployees
                .Include(x => x.Employee)
                .Where(x => x.Task.Id == id
                && x.TaskCapacity.Id == (int)Common.Common.TaskCapacity.Assignee).FirstOrDefault();
            Employee employee = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();

            taskVM.AssigneeCode = currentEmployee.Employee.EmployeeName;
            taskVM.CurrentUserName = currentUserName;
            ViewData["UserName"] = currentUserName;
            ViewData["EmployeeName"] = employee == null ? "" : employee.EmployeeName;
            taskVM.IsReadOnly = viewMode == 1 ? true : false;
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
