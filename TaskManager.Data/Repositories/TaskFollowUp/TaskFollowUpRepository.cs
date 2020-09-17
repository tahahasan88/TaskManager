using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data.Repositories.FollowUpTask
{
    public class TaskFollowUpRepository : ITaskFollowUpRepository
    {
        public readonly TaskManagerContext _context;
        public TaskFollowUpRepository(TaskManagerContext context)
        {
            _context = context;
        }
        public bool AddFollowUpTask()
        {
            throw new NotImplementedException();
        }

        public List<TaskFollowUp> GetFollowUpTasks()
        {
            throw new NotImplementedException();
        }
    }
}
