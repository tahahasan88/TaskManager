using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data.Repositories
{
    public class SubTaskRepository : ISubTaskRepository
    {
        public readonly TaskManagerContext _context;
        public SubTaskRepository(TaskManagerContext context)
        {
            _context = context;
        }
        public List<SubTask> GetAllSubTasksByTaskId(int taskId)
        {
            throw new NotImplementedException();
        }
    }
}
