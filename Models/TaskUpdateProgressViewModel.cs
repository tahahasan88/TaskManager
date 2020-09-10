using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class TaskUpdateProgressViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        [Required]
        public string Remarks { get; set; }
        public string TaskProgress { get; set; }
        public List<string> TaskEmployees { get; set; }
        public List<SelectListItem> StatusList { get; set; }
    }
}
