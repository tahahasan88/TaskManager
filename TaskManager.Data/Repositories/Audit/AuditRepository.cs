using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Repositories.Audit
{
    class AuditRepository : IAuditRepository
    {
        public readonly TaskManagerContext _context;
        public AuditRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAudit(DateTime actionDate, string actionBy, string description, Task task, int progressType, string currentUserName)
        {
            bool isActionAudited = false;
            try
            {
                TaskAudit progressAudit = new TaskAudit();
                progressAudit.ActionDate = DateTime.Now;
                progressAudit.ActionBy = currentUserName;
                progressAudit.Description = description;
                progressAudit.Task = task;
                progressAudit.Type = _context.AuditType.Where(x => x.Id == progressType).SingleOrDefault();
                await _context.AddAsync(progressAudit);
            }
            catch (Exception ex)
            {

            }
            return isActionAudited;
        }
    }
}
