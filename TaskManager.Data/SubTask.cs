using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data
{
    public class SubTask
    {
        public int Id { get; set; }
        public virtual Task Task { get; set; }
        public string Description { get; set; }
        public virtual TaskStatus TaskStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public virtual Employee  LastUpdatedBy { get; set; }
        public virtual Employee SubTaskAssignee { get; set; }
        public bool IsDeleted { get; set; }
    }
}
