using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers
{
    public class BaseController : Controller
    {
        public readonly TaskManagerContext _context;
        public BaseController(TaskManagerContext context)
        {
            _context = context;
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

    }
}