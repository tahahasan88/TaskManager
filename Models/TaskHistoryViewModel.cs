using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data;

namespace TaskManager.Web.Models
{
    public class TaskHistoryViewModel
    {
        public List<TaskAudit> ProgressHistory { get; set; }
        public List<TaskAudit> AssignmentHistory { get; set; }
        public List<TaskAudit> FollowUpHistory { get; set; }
        public List<TaskAudit> FollowUpResponseHistory { get; set; }
        public List<TaskAudit> SubTasksHistory { get; set; }

    }
}
