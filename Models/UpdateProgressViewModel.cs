using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Data;

namespace TaskManager.Web.Models
{
    public class UpdateProgressViewModel
    {
        public Task Task { get; set; }
        public int SelectedStatus { get; set; }
        public List<SelectListItem> StatusList { get; set; }

    }
}
