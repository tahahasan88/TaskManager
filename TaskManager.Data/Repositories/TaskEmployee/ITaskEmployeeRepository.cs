using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Repositories
{
    public interface ITaskEmployeeRepository
    {
        List<TaskEmployee> GetTaskEmployeesByTaskId(int taskId);
        Task<bool> AddTaskEmployee(Task task, int assigneeCapacity, string assigneeUserName);
    }
}
