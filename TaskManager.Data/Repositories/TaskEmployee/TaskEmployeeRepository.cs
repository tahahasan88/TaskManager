using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Repositories
{
    public class TaskEmployeeRepository : ITaskEmployeeRepository
    {
        public readonly TaskManagerContext _context;
        public TaskEmployeeRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public async Task<bool> AddTaskEmployee(Task task, int assigneeCapacity, string assigneeUserName)
        {
            bool isEmployeeAdded = false;
            try
            {
                TaskEmployee taskAssignee = new TaskEmployee();
                taskAssignee.Task = task;
                int taskAssigneeCapacity = (int)TaskManager.Common.Common.TaskCapacity.Assignee;
                taskAssignee.TaskCapacity = _context.TaskCapacities.Where(x => x.Id == taskAssigneeCapacity).SingleOrDefault();
                taskAssignee.Employee = _context.Employees.Where(x => x.UserCode == assigneeUserName).SingleOrDefault();
                await _context.AddAsync(taskAssignee);
                isEmployeeAdded = true;
            }
            catch (Exception ex)
            {
                isEmployeeAdded = false;
            }
            return isEmployeeAdded;
        }

        public async Task<List<TaskEmployee>> GetTaskEmployeesByTaskId(int taskId)
        {
            return  await _context.TaskEmployees
                .Where(x => x.Task.Id == taskId).ToListAsync();
        }

        List<TaskEmployee> ITaskEmployeeRepository.GetTaskEmployeesByTaskId(int taskId)
        {
            throw new NotImplementedException();
        }
    }
}
