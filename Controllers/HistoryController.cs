using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Common;
using TaskManager.Data;
using TaskManager.Web.Models;


namespace TaskManager.Web.Controllers
{
    public class HistoryController
    {
        private readonly TaskManagerContext _context;

        public HistoryController(TaskManagerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> List(int taskId)
        {
            List<HistoryViewModel> auditVMList = new List<HistoryViewModel>();
            List<TaskAudit> auditList = await _context.TaskAudit
                .Include(x => x.Task)
                .Include(x => x.Type)
                .Where(x => x.Task.Id == taskId)
                .OrderByDescending(x => x.ActionDate)
                .ToListAsync();

            foreach (TaskAudit auditTask in auditList)
            {
                auditVMList.Add(new HistoryViewModel()
                {
                    HistoryDate = auditTask.ActionDate,
                    ActionBy = auditTask.ActionBy,
                    Description = auditTask.Description,
                    Type = auditTask.Type.Id
                });
            }
            return new JsonResult(new { records = auditVMList });
        }
    }
}
