using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class TaskEmployeeListViewModel
    {
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public int CapacityId { get; set; }
    }
}
