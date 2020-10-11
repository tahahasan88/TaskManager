using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class SubTaskListViewModel
    {
        public List<AssigneeDropDownViewModel> employeeVMList { get; set; }
        public List<SbTaskViewModel> subTaskVMList { get; set; }
    }
}
