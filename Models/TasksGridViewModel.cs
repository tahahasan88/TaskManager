using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class TasksGridViewModel
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Progress { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedToEmployeeName { get; set; }
        public string LastUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string TargetDate { get; set; }
        public int SortId { get; set; }
        public bool IsEditable { get; set; }
    }
}
