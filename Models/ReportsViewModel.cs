using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class ReportsViewModel
    {
        public string TagName { get; set; }
        public int TagCount { get; set; }
        public int NotStartedTasksCount { get; set; }
        public int InProgressTasksCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public int OnHoldTasksCount { get; set; }
        public int CancelledTasksCount { get; set; }
    }
}
