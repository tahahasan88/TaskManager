using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class TaskSummaryViewModel
    {
        public int PendingTasksCount { get; set; }
        public int OverDueTasksCount { get; set; }
        public int ResolvedTodayCount { get; set; }
        public int FollowUpsCount { get; set; }
    }
}
