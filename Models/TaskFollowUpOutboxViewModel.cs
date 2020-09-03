using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class TaskFollowUpOutboxViewModel
    {
        public DateTime FollowUpDate { get; set; }
        public string Remarks { get; set; }
        public string TaskInfo { get; set; }
        public string Status { get; set; }
    }
}
