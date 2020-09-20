using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data
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
        public string PhoneNo { get; set; }
        public virtual Department Department { get; set; }
    }
}
