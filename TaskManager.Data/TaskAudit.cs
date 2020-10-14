using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data
{
    public class TaskAudit
    {
        public int Id { get; set; }
        public AuditType Type { get; set; }
        public string Description { get; set; }
        public virtual Employee ActionBy { get; set; }
        public Task Task { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
