using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data;

namespace TaskManager.Web.Models
{
    public class TaskFollowUpViewModel
    {
        public TaskFollowUp TaskFollowUp { get; set; }
        public int Id { get; set; }
        [Required(ErrorMessage ="*Remarks are required")]
        public string Remarks { get; set; }
        public string FollowerUserName { get; set; }
        public string ListofTasks { get; set; }
    }
}
