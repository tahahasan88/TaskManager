using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskManager.Data
{
    public class TaskFollowUp
    {
        public int Id { get; set; }
        public string Remarks { get; set; }
        public virtual Task Task { get; set; }
        public virtual Employee Follower { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
