using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TaskManager.Data;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class BaseController : Controller
    {
        public readonly TaskManagerContext _context;
        protected string currentUserName;
        protected IConfiguration _iconfiguration;

        public BaseController(TaskManagerContext context, IConfiguration config)
        {
            _context = context;
            _iconfiguration = config;
        }

        protected void SetUserName()
        {
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["username"]))
            {
                currentUserName = HttpContext.Request.Query["username"];
            }
        }

        public List<string> GetTaskEmployeeEmailAddresses(int taskId, string currentUserName)
        {
            List<TaskEmployee> taskEmployees =
                _context.TaskEmployees.Where(x => x.Task.Id == taskId).ToList();
            List<string> listOfEmployeeEmailAddress = new List<string>();

            //EmployeeList employeeList = new EmployeeList();
            List<Employee> employees = _context.Employees.Include(x => x.Department).ToList();
            foreach (TaskEmployee employee in taskEmployees)
            {
                if (employee.UserName != null && employee.UserName != currentUserName)
                {
                    listOfEmployeeEmailAddress.Add(employees.Where(x => x.UserName == employee.UserName)
                        .SingleOrDefault().EmailAddress);
                }
            }
            return listOfEmployeeEmailAddress;
        }

        protected bool IsThisUserManagingThisDepartment(Department thisDepartment, string userName)
        {
            bool isThisUserManager = false;
            Department parentDepartment = thisDepartment.ParentDepartment;
            if (parentDepartment != null)
            {
                thisDepartment = parentDepartment;
                while (thisDepartment != null)
                {
                    if (thisDepartment.Manager.UserName == userName)
                    {
                        isThisUserManager = true;
                        break;
                    }
                    thisDepartment = thisDepartment.ParentDepartment;
                }
            }
            else
            {
                isThisUserManager = thisDepartment.Manager.UserName == userName;
            }
            return isThisUserManager;
        }

        protected int GetDeptLevel(Department thisDept)
        {
            int depthLevel = 0;
            while (thisDept.ParentDepartment != null)
            {
                thisDept = thisDept.ParentDepartment;
                depthLevel = depthLevel + 2;
            }
            return depthLevel;
        }

        protected void GetUserData(string connectionString, bool isManager, int deptId, Dictionary<string, string> userDataDictionary)
        {
            string sqlQuery = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (isManager)
                {
                    sqlQuery = "select UserName, EmployeeName from employees where DepartmentId = " + deptId +  
                    "union " +
                    "select UserName, EmployeeName from employees where id in (select managerId from Departments where ParentDepartmentId = " + deptId + ") " +
                    "union " +
                    "select UserName, EmployeeName from employees where id in (select managerId from Departments where ParentDepartmentId = " +
                    "(select ParentDepartmentId from Departments where Id = " + deptId + "))";
                }
                else
                {
                    sqlQuery = "select UserName, EmployeeName from employees where DepartmentId = " + deptId + "";
                }

                conn.Open();
                var queryResult = conn.Query(sqlQuery);
                foreach (var result in queryResult)
                {
                    if (userDataDictionary.ContainsKey(result.UserName))
                    {
                        userDataDictionary[result.UserName].Add(result.EmployeeName);
                    }
                    else
                    {
                        userDataDictionary.Add(result.UserName, result.EmployeeName);
                    }
                }
            }
        }

        protected List<AssigneeDropDownViewModel> LoadAssigneeDrpDwnData(string username)
        {
            var connectionString = _iconfiguration.GetConnectionString("DefaultConnection");
            Dictionary<string, string> userDataDictionary = new Dictionary<string, string>();
            int tagCounter = 0;

            List<Employee> employees = _context.Employees.Include(x => x.Department).ToList();
            List<Department> departments = _context.Departments
                .Include(x => x.ParentDepartment)
                .Include(x => x.Manager).OrderBy(x => x.Id).ToList();

            bool isManager = employees.Where(x => x.UserName == username && x.Department.Manager.Id == x.Id).Any();
            int deptId = employees.Where(x => x.UserName == username).Select(x=>x.Department.Id).FirstOrDefault();

            GetUserData(connectionString, isManager, deptId, userDataDictionary);

            List<AssigneeDropDownViewModel> assigneeVM = new List<AssigneeDropDownViewModel>();

            foreach (Department department in departments)
            {
                AssigneeDropDownViewModel assigneeRow = new AssigneeDropDownViewModel();
                assigneeRow.TagName = department.Name;
                assigneeRow.TagId = ++tagCounter;
                assigneeRow.IsDeptName = true;
                assigneeRow.DepartmentLevel = GetDeptLevel(department);
                assigneeVM.Add(assigneeRow);

                foreach (Employee employee in employees.Where(x => x.Department.Id == department.Id))
                {
                    if (userDataDictionary.ContainsKey(employee.UserName))
                    {
                        CreateAssigneeRows(assigneeVM, employee, ++tagCounter, assigneeRow.DepartmentLevel);
                    }
                }
            }


            return assigneeVM;
        }

        private void CreateAssigneeRows(List<AssigneeDropDownViewModel> assigneeVM, Employee employee, int tagId, int deptLevel)
        {
            AssigneeDropDownViewModel newRow = new AssigneeDropDownViewModel();
            newRow.TagName = employee.EmployeeName;
            newRow.TagId = tagId;
            newRow.DepartmentLevel = deptLevel;
            newRow.TagUserName = employee.UserName;
            assigneeVM.Add(newRow);
        }

    }
}