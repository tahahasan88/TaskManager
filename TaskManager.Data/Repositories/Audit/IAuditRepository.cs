using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Repositories.Audit
{
    public interface IAuditRepository
    {
        Task<bool> AddAudit(DateTime actionDate, string actionBy, string description, Task task, int progressType, string currentUserName);
    }
}
