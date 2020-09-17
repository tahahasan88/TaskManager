using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data.Repositories.FollowUpTask
{
    public interface ITaskFollowUpRepository
    {
        bool AddFollowUpTask();
        List<TaskFollowUp> GetFollowUpTasks();
    }
}
