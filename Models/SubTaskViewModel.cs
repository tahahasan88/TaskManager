using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data;

namespace TaskManager.Web.Models
{
    public class SubTaskViewModel
    {
        public SubTask SubTask { get; set; }
        public bool IsCompleted { get; set; }
        public string SelectedEmployee { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
    }
}
