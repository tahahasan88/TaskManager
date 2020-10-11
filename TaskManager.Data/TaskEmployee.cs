using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data
{
    public class TaskEmployee
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public virtual Task Task { get; set; }
        public virtual TaskCapacity TaskCapacity { get; set; }
        public bool IsActive { get; set; }
        public virtual Employee Employee { get; set; }

    }

}
