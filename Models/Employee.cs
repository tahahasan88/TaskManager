using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class InternalEmployee
    {
        public int Id { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string EmployeeName { get; set; }
        public string EmailAddress { get; set; }
        public string JobTitle { get; set; }
        public string RegistrationNo { get; set; }
        public string PhoneNo { get; set; }

    }

    public class EmployeeList
    {
        public List<InternalEmployee> Employees;
        public EmployeeList()
        {
            Employees = new List<InternalEmployee>();
            Employees.Add(new InternalEmployee() { Id = 1, UserName = "tahahasan", PhoneNo = "0501273980", UserCode = "tahahasan", EmployeeName = "Taha", EmailAddress = "taha.hasan88@gmail.com" });
            Employees.Add(new InternalEmployee() { Id = 2, UserName = "Dawar", PhoneNo = "0501273981", UserCode = "Dawar", EmployeeName = "Dawar", EmailAddress = "dawar@gmail.com" });
            Employees.Add(new InternalEmployee() { Id = 3, UserName = "Kashif", PhoneNo = "0501273982", UserCode = "Kashif", EmployeeName = "Kashif", EmailAddress = "kashif@gmail.com" });
            Employees.Add(new InternalEmployee() { Id = 4, UserName = "Hasan", PhoneNo = "0501273983", UserCode = "Hasan", EmployeeName = "Hasan", EmailAddress = "taha.hasan88@gmail.com" });
            Employees.Add(new InternalEmployee() { Id = 5, UserName = "Kamran", PhoneNo = "0501273984", UserCode = "Kamran", EmployeeName = "Kamran", EmailAddress = "kamran@gmail.com" });
        }
    }
}
