using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Data
{
    public class Task
    {
        public int Id { get; set; }
        public string TaskProgress { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Target { get; set; }  
        public virtual TaskStatus TaskStatus { get; set; }
        public bool IsDeleted { get; set; }
        public virtual TaskPriority TaskPriority { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        [ForeignKey("TaskEmployee")]
        public string LastUpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
