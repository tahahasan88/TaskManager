using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class ReportsViewModel
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string TagUserName { get; set; }
        public int TagCount { get; set; }
        public int NotStartedTasksCount { get; set; }
        public int InProgressTasksCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public int OnHoldTasksCount { get; set; }
        public int CancelledTasksCount { get; set; }
        public bool IsDeptName { get; set; }
        public int DepartmentLevel { get; set; }
    }
}
