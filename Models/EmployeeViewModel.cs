using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class EmployeeViewModel
    {
        public string EmployeeName { get; set; }
        public List<string> ReportsTo { get; set; }
        public string Presence { get; set; }
        public string Phone { get; set; }
        public string EmailAddres { get; set; }
        public List<TaskFollowUpInboxViewModel> TasksFollowUpVMList { get; set; }
        public List<TasksGridViewModel> TasksGridVMList { get; set; }


    }
}
