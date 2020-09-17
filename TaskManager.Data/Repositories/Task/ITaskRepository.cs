using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Repositories
{
    public interface ITaskRepository
    {
        List<Task> GetAllTasks();
        Task<Task> GetTaskById(int id);
        Task<bool> AddTask(string currentUserName, DateTime targetDate, string title, string description, string taskProgress, int priorityId, int taskStatusId);
        bool UpdateTask(Task task);
        List<Task> GetPendingTasks();
        List<Task> GetOVerDueTasks();

    }
}
