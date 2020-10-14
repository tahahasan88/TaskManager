using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Common;

namespace TaskManager.Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        public readonly TaskManagerContext _context;
        public TaskRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public async Task<bool> AddTask(string currentUserName, DateTime targetDate, string title, string description, string taskProgress, int priorityId, int taskStatusId)
        {
            bool isTaskAdded = false;
            try
            {
                Data.Task task = new Data.Task();
                task.IsDeleted = false;
                task.LastUpdatedAt = DateTime.Now;
                task.LastUpdatedBy = _context.Employees.Where(x => x.UserCode == currentUserName).SingleOrDefault();
                task.CreatedAt = DateTime.Now;
                task.Target = targetDate;
                task.Title = title;
                task.Description = description;
                task.TaskProgress = taskProgress;
                task.TaskPriority = _context.TaskPriority.Where(x => x.Id == priorityId).SingleOrDefault();
                task.TaskStatus = _context.TaskStatus.Where(x => x.Id == taskStatusId).SingleOrDefault();
                _context.Add(task);
                await _context.SaveChangesAsync();
                isTaskAdded = true;
            }
            catch (Exception ex)
            {
                isTaskAdded = false;
            }
            return isTaskAdded;
        }

        public bool AddTask()
        {
            throw new NotImplementedException();
        }

        public List<Task> GetAllTasks()
        {
            IEnumerable<TaskCapacity> capacities = _context.TaskCapacities;
            foreach (TaskCapacity capacity in capacities)
            {

            }

            return new List<Task>();
        }

        public List<Task> GetOVerDueTasks()
        {
            throw new NotImplementedException();
        }

        public List<Task> GetPendingTasks()
        {
            throw new NotImplementedException();
        }

        public async Task<Task> GetTaskById(int id)
        {
            return  await _context.Tasks.Where(x => x.Id == id).SingleOrDefaultAsync();
        }

        public bool UpdateTask(Task task)
        {
            throw new NotImplementedException();
        }
    }
}
