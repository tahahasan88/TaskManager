using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TaskManager.Common.Common;

namespace TaskManager.Web
{
    public class TaskPermissions
    {
        public static bool IsAllowed(TaskAction taskAction, TaskCapacity capacity)
        {
            bool isAllowed = false;
            if (taskAction == TaskAction.TaskCreate && capacity == TaskCapacity.Creator)
            {
                isAllowed = true;
            }
            else if (taskAction == TaskAction.TaskDelete && (capacity == TaskCapacity.Creator))
            {
                isAllowed = true;
            }
            else if (taskAction == TaskAction.TaskEdit && (capacity == TaskCapacity.Assignee))
            {
                isAllowed = true;
            }
            else if (taskAction == TaskAction.SubTaskCreate && ((capacity == TaskCapacity.Assignee) || (capacity == TaskCapacity.Creator)
                || (capacity == TaskCapacity.Follower)))
            {
                isAllowed = true;
            }
            else if (taskAction == TaskAction.SubTaskDelete && ((capacity == TaskCapacity.Assignee) || (capacity == TaskCapacity.Creator) || (capacity == TaskCapacity.Follower)))
            {
                isAllowed = true;
            }
            else if (taskAction == TaskAction.SubTaskEdit && ((capacity == TaskCapacity.Assignee)))
            {
                isAllowed = true;
            }
            else if (taskAction == TaskAction.ProgressUpdate && capacity == TaskCapacity.Assignee)
            {
                isAllowed = true;
            }
            else if (taskAction == TaskAction.AssigneeUpdate && (capacity == TaskCapacity.Assignee || capacity == TaskCapacity.Creator))
            {
                isAllowed = true;
            }
            return isAllowed;
        }
    }
}
