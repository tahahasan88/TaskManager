using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class SbTaskViewModel
    {
        public int TaskId { get; set; }
        public int SubTaskId { get; set; }
        public string Description { get; set; }
        public string SubTaskUserName { get; set; }
        public string SubTaskEmployeeName { get; set; }
        public bool IsCompleted { get; set; }
        public string AvatarImage { get; set; }
    }
}
