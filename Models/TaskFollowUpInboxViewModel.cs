using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class TaskFollowUpInboxViewModel
    {
        public string UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string TaskInfo { get; set; }
        public string Status { get; set; }
        public string FollowUpFrom { get; set; }
        public string FollowUpEmployeeName { get; set; }
        public int TaskId { get; set; }
        public int SortId { get; set; }
    }
}
