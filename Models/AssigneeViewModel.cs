using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class AssigneeViewModel
    {
        public List<SelectListItem> Assignees { get; set; }
        public string SelectedAssignee { get; set; }
        public int AssigneeTaskId { get; set; }
    }
}
