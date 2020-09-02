using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string EmployeeName { get; set; }
        public string EmailAddress { get; set; }
        public string JobTitle { get; set; }
        public string RegistrationNo { get; set; }

    }

    public class EmployeeList
    {
        public List<Employee> Employees;
        public EmployeeList()
        {
            Employees = new List<Employee>();
            Employees.Add(new Employee() { Id = 1, UserName = "tahahasan", UserCode = "tahahasan", EmployeeName = "Taha", EmailAddress = "taha.hasan88@gmail.com" });
            Employees.Add(new Employee() { Id = 2, UserName = "Dawar", UserCode = "Dawar", EmployeeName = "Dawar", EmailAddress = "dawar@gmail.com" });
            Employees.Add(new Employee() { Id = 3, UserName = "Kashif", UserCode = "Kashif", EmployeeName = "Kashif", EmailAddress = "kashif@gmail.com" });
            Employees.Add(new Employee() { Id = 4, UserName = "Hasan", UserCode = "Hasan", EmployeeName = "Hasan", EmailAddress = "hasan@gmail.com" });
            Employees.Add(new Employee() { Id = 5, UserName = "Kamran", UserCode = "Kamran", EmployeeName = "Kamran", EmailAddress = "kamran@gmail.com" });
        }
    }
}
