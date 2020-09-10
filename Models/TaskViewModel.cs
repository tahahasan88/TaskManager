using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Data;

namespace TaskManager.Web.Models
{
    public class TaskViewModel
    {
        public Task Task { get; set; }
        public int PriorityId { get; set; }
        public string AssigneeCode { get; set; }
        public string CurrentUserName { get; set; }
        public List<SelectListItem> PriorityList { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
    }
}
