using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data
{
    public class TaskFollowUpResponse
    {
        public int Id { get; set; }
        public virtual Task Task { get; set; }
        public string TaskResponse { get; set; }
        public virtual Employee RespondedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
