using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Repositories.TaskFollowUpEmployee
{
    public interface ITaskFollowUpEmployeeRepository
    {
        Task<bool> UpdateTaskFollowUpResponse(Data.Task thisTask, string taskProgress, string currentUserName);
        bool AddFollowUpToTask(int taskId, int employeeId);
    }
}
