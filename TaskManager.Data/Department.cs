using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskManager.Data
{
    public class Department
    {
        public int Id { get; set; }
        public virtual Department ParentDepartment { get; set; }
        public string Name { get; set; }
        [ForeignKey("ManagerId")]
        public virtual Employee Manager { get; set; }
    }
}
