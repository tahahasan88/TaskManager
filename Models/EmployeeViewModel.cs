﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Web.Models
{
    public class EmployeeViewModel
    {
        public string EmployeeName { get; set; }
        public string ReportsTo { get; set; }
        public string Presence { get; set; }
        public string Phone { get; set; }
        public string EmailAddres { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string AvatarImage { get; set; }
        public List<TaskFollowUpInboxViewModel> TasksFollowUpVMList { get; set; }
        public List<TasksGridViewModel> TasksGridVMList { get; set; }


    }
}
