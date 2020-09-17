using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data
{
    public class TaskFollowUp
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public virtual Task Task { get; set; }
        public string FollowerUserName { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
