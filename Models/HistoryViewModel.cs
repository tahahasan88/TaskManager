using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data;

namespace TaskManager.Web.Models
{
    public class HistoryViewModel
    { 
        public string HistoryDate { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public string ActionBy { get; set; }
    }
}
