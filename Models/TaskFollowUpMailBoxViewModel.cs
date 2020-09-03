using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data;

namespace TaskManager.Web.Models
{
    public class TaskFollowUpMailBoxViewModel
    {
        public List<TaskFollowUpInboxViewModel> InBoxFollowUpList { get; set; }
        public List<TaskFollowUpOutboxViewModel> OutBoxFollowUpList { get; set; }
    }
}
