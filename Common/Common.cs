using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Common
{

    public class Common
    {
        
        public enum TaskCapacity
        {
            Creator = 1,
            Assignee = 2,
            Follower = 3,
            ExTaskAssignee = 4,

            SubTaskAssignee = 5
        }

        public static string GetTaskCapacityDescription(int  taskCapacityId)
        {
            return taskCapacityId == 1 ? "Creator" :
                taskCapacityId == 2 ? "Assignee" :
                taskCapacityId == 3 ? "Follower" :
                taskCapacityId == 4 ? "Ex-Task Assignee" :
                "Sub Task Assignee";
        }

        public enum TaskPriority
        {
            Low = 1,
            Medium = 2,
            High = 3
        }


        public enum TaskStatus
        {
            NotStarted = 1,
            InProgress = 2,
            Completed = 3,
            OnHold = 4,
            Cancelled = 5,
            AllTasks = 0
        }

        public static string GetTaskStatusDescription(int taskStatusId)
        {
            return taskStatusId == 1 ? "Not Started" :
                taskStatusId == 2 ? "In Progress" :
                taskStatusId == 3 ? "Completed" :
                taskStatusId == 4 ? "On Hold" :
                taskStatusId == 5 ? "Cancelled" :
                "All Tasks";
        }

        public enum TaskAction
        {
            TaskCreate = 1,
            TaskEdit = 2,
            TaskDelete = 3,
            SubTaskCreate = 4,
            SubTaskEdit = 5,
            SubTaskDelete = 6,
            TaskFollowUp = 7,
            ProgressUpdate = 8,
            AssigneeUpdate = 9
        }

        public enum AuditType
        {
            Progress = 1,
            Assignment = 2,
            FollowUp = 3,
            FollowUpResponse = 4,
            SubTasks = 5,
            Update = 6
        }

        public class ValidateDateRange : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                DateTime thisVal = (DateTime)value;
                // your validation logic
                if (thisVal.Date >= DateTime.Now.Date)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Date is not valid");
                }
            }
        }
    }
}
