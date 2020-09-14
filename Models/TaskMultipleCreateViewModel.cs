using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data;
using static TaskManager.Common.Common;

namespace TaskManager.Web.Models
{
    public class TaskMultipleCreateViewModel
    {
        [Required(ErrorMessage = "Task titles are required")]
        [TaskUniqueValidation]
        public string ListOfTaskTitles { get; set; }
        public string TaskProgress { get; set; }
        public string Description { get; set; }
        [ValidateDateRange]
        public DateTime Target { get; set; }
        public int PriorityId { get; set; }
        public string AssigneeCode { get; set; }

        public class TaskUniqueValidation : ValidationAttribute
        {
            private TaskManagerContext _context { get; set; }

            public TaskUniqueValidation()
            {
                _context = new TaskManagerContext();
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                string taskCommaSeparatedValues = (string)value;
                if (taskCommaSeparatedValues != null && taskCommaSeparatedValues.Trim().Length > 0)
                {
                    string[] taskTitles = taskCommaSeparatedValues.Split(",");
                    string errorMessage = string.Empty;

                    if (taskTitles != null && taskTitles.Length > 0)
                    {
                        foreach (string taskTitle in taskTitles)
                        {
                            Data.Task existingTask = _context.Tasks.Where(x => x.Title == taskTitle && x.IsDeleted == false).SingleOrDefault();
                            bool isTaskAlreadyAdded = existingTask != null ? true : false;

                            if (isTaskAlreadyAdded)
                            {
                                errorMessage += existingTask.Title + ",";
                            }
                        }
                    }
                    if (errorMessage.Equals(string.Empty))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        errorMessage = errorMessage.Substring(0, errorMessage.Length - 1);
                        return new ValidationResult("Following Titles already exist :" + errorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }
    }
}
