using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data.Repositories
{
    public interface ISubTaskRepository
    {
        List<SubTask> GetAllSubTasksByTaskId(int taskId);
    }
}
