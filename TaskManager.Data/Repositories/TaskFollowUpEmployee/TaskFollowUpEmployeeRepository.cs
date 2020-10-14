using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Repositories.TaskFollowUpEmployee
{
    public class TaskFollowUpEmployeeRepository : ITaskFollowUpEmployeeRepository
    {
        public readonly TaskManagerContext _context;
        public TaskFollowUpEmployeeRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public bool AddFollowUpToTask(int taskId, int employeeId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTaskFollowUpResponse(Task thisTask, string taskProgress, string currentUserName)
        {
            bool isFollowUpAdded = false;
            try
            {
                TaskFollowUpResponse taskFollowUpResponse = new TaskFollowUpResponse();
                taskFollowUpResponse.RespondedBy = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
                taskFollowUpResponse.LastUpdatedAt = DateTime.Now;
                taskFollowUpResponse.CreatedAt = DateTime.Now;
                taskFollowUpResponse.TaskResponse = taskProgress;
                taskFollowUpResponse.Task = thisTask;
                await _context.AddAsync(taskFollowUpResponse);
                isFollowUpAdded = true;
            }
            catch (Exception ex)
            {
                isFollowUpAdded = false;
            }
            return isFollowUpAdded;
        }

    }
}
